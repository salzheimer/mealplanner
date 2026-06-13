# Database-per-Service Migration Plan

## Context

All three services currently share a single PostgreSQL database (`mealplannerdb`). Moving to database-per-service enforces true service isolation — no service can accidentally query another's tables, and each DB schema evolves independently. The EF entity models and DbContexts are already scoped correctly (no cross-service navigation properties), so the work is almost entirely in SQL DDL and Docker Compose config.

---

## Recommended Topology: Single Postgres Container, 3 Databases

Use one postgres:17 container with three separate databases (`identity_db`, `meal_recipe_db`, `plan_db`). Standard postgres prevents cross-database SQL queries, giving the same schema isolation as separate containers — with none of the added complexity (volume management, multi-container healthchecks, startup ordering).

---

## Table Groupings

| Database | Tables |
|---|---|
| **identity_db** | `user`, `user_credential`, `password_reset`, `session`, `audit_log`, `group`, `group_member`, `resource_permission` + lookups: `client_types`, `group_member_roles`, `group_member_statuses`, `permissions`, `resource_types`, `subject_types` |
| **meal_recipe_db** | `recipe`, `recipe_instructions`, `recipe_ingredients`, `meal`, `meal_item` + lookups: `meal_types`, `item_types` |
| **plan_db** | `plan`, `meal_plan`, `meal_item_plan` + lookup: `item_statuses` |

---

## Cross-Boundary FK Constraints to Remove

These exist in `init.sql` today as `REFERENCES` clauses but cross service boundaries — they become plain `INT` columns with no constraint in the split files.

**In `meal_recipe_db/init.sql`** (all user references):
- `recipe`: `owner_user_id`, `created_by`, `updated_by`
- `recipe_instructions`: `created_by`, `updated_by`
- `recipe_ingredients`: `created_by`, `updated_by`
- `meal`: `owner_user_id`, `created_by`, `updated_by`
- `meal_item`: `created_by`, `updated_by`

**In `plan_db/init.sql`** (user references + cross-service meal references):
- `plan`: `owner_user_id`, `created_by`, `updated_by`
- `meal_plan`: `meal_id` ← cross-DB to `meal` table; `added_by_user_id`, `created_by`, `updated_by`
- `meal_item_plan`: `meal_item_id` ← cross-DB to `meal_item` table; `assigned_to_user`, `created_by`, `updated_by`

All same-service FKs (`recipe_instructions.recipe_id → recipe`, `meal_plan.plan_id → plan`, `meal_item_plan.meal_plan_id → meal_plan`) are retained.

---

## Implementation Steps

### Phase 1 — SQL Split

1. **Create `infrastructure/postgres/identity_db/init.sql`** — copy identity tables from `init.sql`. Fix bugs present in the original:
   - `session.user_id` references `"user"(id)` → fix to `REFERENCES "user"(id)` (the table uses quoted `"user"`, which is fine, just ensure the column ref is `(id)` matching the PK)
   - `audit_log.session_id REFERENCES sessions(id)` → table is named `session`, not `sessions`

2. **Create `infrastructure/postgres/meal_recipe_db/init.sql`** — copy meal/recipe tables. Drop all `REFERENCES "user"(id)` clauses (listed above). Keep all intra-service FKs. Add `meal_share` and `recipe_share` table definitions (they exist in `MealDbContext` but are absent from `init.sql`).

3. **Create `infrastructure/postgres/plan_db/init.sql`** — copy plan tables. Drop cross-DB FKs (`meal_plan.meal_id`, `meal_item_plan.meal_item_id`, all user refs). Keep `meal_plan.plan_id → plan` and `meal_item_plan.meal_plan_id → meal_plan`.

4. **Create `infrastructure/postgres/init-all.sh`** — shell script that creates all three databases then applies each init file:
   ```bash
   #!/bin/bash
   set -e
   psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
       CREATE DATABASE meal_recipe_db;
       CREATE DATABASE plan_db;
   EOSQL
   psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" -d identity_db \
       -f /docker-entrypoint-initdb.d/identity_db/init.sql
   psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" -d meal_recipe_db \
       -f /docker-entrypoint-initdb.d/meal_recipe_db/init.sql
   psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" -d plan_db \
       -f /docker-entrypoint-initdb.d/plan_db/init.sql
   ```
   Note: `POSTGRES_DB=identity_db` so the primary DB is auto-created; the script creates the other two.

5. Make `init-all.sh` executable: `chmod +x infrastructure/postgres/init-all.sh`

### Phase 2 — Docker Compose Update

6. **Update `infrastructure/docker/docker-compose.yml`**:

   **postgres service** — change env and volumes:
   ```yaml
   environment:
     POSTGRES_USER: mealplanner_user
     POSTGRES_PASSWORD: mealPlanner26!
     POSTGRES_DB: identity_db        # was mealplannerdb
   volumes:
     - mealplanningdb_data:/var/lib/postgresql/data
     - ../postgres/init-all.sh:/docker-entrypoint-initdb.d/init-all.sh
     - ../postgres/identity_db/init.sql:/docker-entrypoint-initdb.d/identity_db/init.sql
     - ../postgres/meal_recipe_db/init.sql:/docker-entrypoint-initdb.d/meal_recipe_db/init.sql
     - ../postgres/plan_db/init.sql:/docker-entrypoint-initdb.d/plan_db/init.sql
   healthcheck:
     test: ["CMD-SHELL", "pg_isready -U $${POSTGRES_USER} -d identity_db"]
   ```

   **Per-service connection strings** (change `Database=` value only):
   - `auth-service`: `Database=identity_db`
   - `meal-service`: `Database=meal_recipe_db`
   - `plan-service`: `Database=plan_db`

   Also add to `plan-service` environment:
   ```yaml
   - MealRecipeService__BaseUrl=http://meal-service
   ```

### Phase 3 — PlanService HTTP Client (for meal existence validation)

7. **Create `services/PlanService/src/Clients/IMealRecipeServiceClient.cs`**:
   ```csharp
   public interface IMealRecipeServiceClient
   {
       Task<Result<MealDto>> GetMealByIdAsync(int mealId);
   }
   ```

8. **Create `services/PlanService/src/Clients/MealRecipeServiceClient.cs`** — follow the exact pattern of `IdentityServiceClient.cs`: constructor reads `configuration["MealRecipeService:BaseUrl"]`, uses `ServiceClient.CreateClient`, forwards Bearer token. Calls `GET /api/meal/{mealId}`.

9. **Register in `services/PlanService/src/Program.cs`**:
   ```csharp
   builder.Services.AddScoped<IMealRecipeServiceClient, MealRecipeServiceClient>();
   ```

10. **Add to `services/PlanService/src/appsettings.json`**:
    ```json
    "MealRecipeService": { "BaseUrl": "http://meal-service" }
    ```

11. **Wire into `MealPlanService`**: Inject `IMealRecipeServiceClient`. On `CreateMealPlanAsync`, call `GetMealByIdAsync(dto.MealId)` before insert; return `MealPlanErrors.NotFound` if the meal service returns a failure.

### Phase 4 — CLAUDE.md Update

12. Update the **Database** section in `mealplanner/CLAUDE.md` to list all three databases and point schema source-of-truth to the three separate init files.

---

## Files Changed

| File | Action |
|---|---|
| `infrastructure/postgres/identity_db/init.sql` | Create |
| `infrastructure/postgres/meal_recipe_db/init.sql` | Create |
| `infrastructure/postgres/plan_db/init.sql` | Create |
| `infrastructure/postgres/init-all.sh` | Create |
| `infrastructure/postgres/init.sql` | Delete |
| `infrastructure/docker/docker-compose.yml` | Modify |
| `services/PlanService/src/Clients/IMealRecipeServiceClient.cs` | Create |
| `services/PlanService/src/Clients/MealRecipeServiceClient.cs` | Create |
| `services/PlanService/src/Program.cs` | Modify |
| `services/PlanService/src/appsettings.json` | Modify |
| `mealplanner/CLAUDE.md` | Modify |

**No EF entity or DbContext files need changes** — cross-service references are already plain `int` columns with no navigation properties.

---

## Verification

1. **Destroy and recreate the volume** (init.sql only runs on a fresh data directory):
   ```bash
   docker volume rm mealplanningdb_data
   docker volume create mealplanningdb_data
   ```

2. **Start the stack**:
   ```bash
   docker compose -f infrastructure/docker/docker-compose.yml up --build
   ```

3. **Confirm all three databases exist**:
   ```bash
   docker exec -it <postgres-container> psql -U mealplanner_user -c "\l"
   # Should list: identity_db, meal_recipe_db, plan_db
   ```

4. **Confirm each service is healthy**: `GET /health` on ports 5002, 5003, 5004.

5. **Smoke test cross-service boundary**:
   - Register a user (IdentityService) → get JWT
   - Create a meal (MealRecipeService)
   - Add that meal to a plan (PlanService) — this exercises the new `MealRecipeServiceClient` validation
   - Retrieve the plan — should return `meal_id` as an integer
