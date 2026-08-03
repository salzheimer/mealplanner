# Meal Planning Microservices

.NET 10 microservices system for meal planning. Services communicate over HTTP via Docker Compose DNS. JWT authentication is issued by IdentityService and validated by downstream services.

---

## Essential Commands

### Run the full stack
```bash
# From workspace root — create the external volumes once if they don't exist
docker volume create identitydb_data
docker volume create mealrecipedb_data
docker volume create plandb_data

# Start all services
docker compose -f infrastructure/docker/docker-compose.yml up --build
```

### Build a single service locally
```bash
dotnet build services/PlanService/src/PlanService.csproj
dotnet build services/MealRecipeService/src/MealRecipeService.csproj
dotnet build services/IdentityService/src/IdentityService.csproj
dotnet build services/ApiGateway/src/ApiGateway.csproj
dotnet build services/GatewayBff/src/GatewayBff.csproj
```

### Run a single service locally
```bash
cd services/PlanService/src && dotnet run
```

### View logs
```bash
docker compose -f infrastructure/docker/docker-compose.yml logs -f <service-name>
# service names: identity_db, meal_recipe_db, plan_db, rabbitmq,
#                identity_service, meal_recipe_service, plan_service,
#                gateway_bff, api_gateway, frontend
```

---

## Service Ports (host → container)

| Service        | Host Port | Docs / IDE                              |
|----------------|-----------|------------------------------------------|
| frontend       | 3000      | —                                        |
| api_gateway    | 5001      | http://localhost:5001/scalar/v1          |
| identity_service | 5002    | http://localhost:5002/scalar/v1          |
| meal_recipe_service | 5003 | http://localhost:5003/scalar/v1          |
| plan_service   | 5004      | http://localhost:5004/scalar/v1          |
| gateway_bff    | 5005      | http://localhost:5005/graphql (Banana Cake Pop, Development only) |
| identity_db    | 5433      | —                                        |
| meal_recipe_db | 5434      | —                                        |
| plan_db        | 5435      | —                                        |

OpenAPI JSON: `http://localhost:<port>/openapi/v1.json`
Health check: `http://localhost:<port>/health`

`frontend` (nginx) is the single entry point for browser traffic — it proxies `/api/*` and `/graphql` to `api_gateway`, which then routes to the individual services. See "Request Routing" below for the full path table.

---

## Project Structure

```
.
├── CLAUDE.md                        # This file
├── meal_planning_monorepo.sln       # Visual Studio solution file
├── docs/
│   ├── design-patterns.md
│   └── plans/                       # Implementation plans (gateway, db-per-service, etc.)
├── infrastructure/
│   ├── docker/
│   │   └── docker-compose.yml       # Orchestration for all services + postgres + rabbitmq
│   └── postgres/
│       ├── identity_db/init.sql     # Authoritative schema — identity_db
│       ├── meal_recipe_db/init.sql  # Authoritative schema — meal_recipe_db
│       └── plan_db/init.sql         # Authoritative schema — plan_db
├── services/
│   ├── ApiGateway/src/              # YARP reverse proxy — routes /api/* and /graphql, no DB connection
│   ├── GatewayBff/src/              # GraphQL BFF — composes cross-service views (dashboard, etc.)
│   ├── IdentityService/src/             # JWT issuance, user registration/login
│   ├── MealRecipeService/src/             # Recipes, ingredients, meal scheduling
│   └── PlanService/src/             # Meal plans and meal item tracking
└── shared/
    ├── Shared.Models/               # DTOs, enums, JwtSettings record
    ├── Shared.Services/             # TokenService, UserService, ServiceClient
    └── Shared.Tests/
```

---

## Architecture

- **IdentityService** issues JWT tokens. All other services validate JWTs using Bearer auth middleware configured with the same `JwtSettings`.
- **ApiGateway** is a YARP reverse proxy — it routes `/api/*` and `/graphql` to the owning service and does not connect to any database. It is the only service the frontend talks to.
- **GatewayBff** is a GraphQL BFF for cross-service composite views (e.g. the dashboard, which needs meal plans from PlanService joined with meals from MealRecipeService). It calls the existing REST APIs over HTTP — it is not a database client either.
- **IdentityService**, **MealRecipeService**, and **PlanService** each own a separate Postgres database (`identity_db`, `meal_recipe_db`, `plan_db`) and connect via their own `ConnectionStrings__*` variable (injected via docker-compose environment). See `docs/plans/meal-planner-db-per-service-transition-plan.md`.
- Services discover each other by Docker Compose service name (e.g. `http://meal_recipe_service/api/meal`).
- All services expose `GET /health` returning `{ "status": "Healthy" }`.

### Request routing (frontend → api_gateway → services)

| Path                  | Routed to             |
|------------------------|------------------------|
| `/api/auth/*`           | `identity_service`     |
| `/api/recipes/*`, `/api/meal/*` | `meal_recipe_service` |
| `/api/plans/*`, `/api/mealplan/*` | `plan_service`   |
| `/graphql`              | `gateway_bff`           |

This table is the single source of truth for both `api_gateway`'s YARP config (`services/ApiGateway/src/appsettings.json`) and `frontend/nginx.conf` today, and is what an Azure APIM import would mirror later — see `docs/plans/api-gateway-hybrid-plan.md`.

---

## Database

- **Engine:** PostgreSQL 17
- **Databases:** `identity_db`, `meal_recipe_db`, `plan_db` — one per owning service, no cross-database queries.
- **User:** `mealplanner_user`
- **Schema source of truth:** `infrastructure/postgres/{identity_db,meal_recipe_db,plan_db}/init.sql` — edit the file for the relevant service, not individual service code.
- The volumes `identitydb_data`, `mealrecipedb_data`, `plandb_data` are external — create them manually before first run (see Essential Commands above). Each `init.sql` only runs on first initialization of its volume.

---

## Shared Models (`Shared.Models` namespace)

Enums mirror the postgres enum types exactly:
- `MealType`: Breakfast, Lunch, Dinner, Snack
- `ItemType`: Recipe, Homemade, StoreBought
- `ItemStatus`: Unknown, Pending, Confirmed

`JwtSettings` is a record: `(Issuer, Audience, Secret, ExpiresMinutes)`

---

## Code Conventions

- Use **records** for DTOs and settings (immutable by default).
- All new enums and DTOs go in `Shared.Models` — not in individual services — so all services share consistent contracts.
- Controllers use `[ApiController]` + `[Route("api/[controller]")]`.
- Endpoints requiring auth use `[Authorize]`.
- Health endpoint is always `GET /health` and is excluded from auth.
- Scalar UI is only mapped in `Development` environment (inside `if (app.Environment.IsDevelopment())`).

---

## Constraints

- Do not create a `postgres` role — the superuser is `mealplanner_user`. External tools connecting to port 5432 must use this user.
- Do not add DB connection code to **ApiGateway** — it routes only.
- Do not use Swagger/Swashbuckle — this project uses `Microsoft.AspNetCore.OpenApi` + `Scalar.AspNetCore`.
- Do not put shared types (DTOs, enums) inside individual service projects.
- The `init.sql` schema uses snake_case column names and postgres-native enum types. C# enums in `Shared.Models` map to these — keep them in sync when schema changes.

---

## API & Controller Standards

These rules apply to all controller work. Flag violations during code review or editing, not only when running `/assess-controllers`.

### Return types
Controllers must return `IActionResult` via `HandleResult<T>()`, never `Result<T>` directly.
Returning `Result<T>` causes all failures to respond with HTTP 200.

### Error namespaces — use the correct type per service
- `PlansController` → `PlanningErrors`
- `MealPlanController` → `MealPlanErrors`
- `MealController` → `MealErrors`
- `RecipesController` → `RecipeErrors`
- `AuthController` / `PermissionController` → `UserErrors` / `PermissionErrors`

Mixing namespaces (e.g., `MealPlanErrors.Unauthorized` in `PlansController`) produces wrong error codes on the wire.

### Authorization
`[Authorize]` on the controller class covers all methods. Do not repeat it on individual action methods unless an action intentionally requires *different* auth (e.g., `[AllowAnonymous]` on a public endpoint within an otherwise-protected controller).

### Route conventions
- No verbs in URL segments. Use HTTP method + noun: `POST /ingredients`, not `POST /add-ingredient`.
- PUT and DELETE must include the resource ID in the route template, not only in the DTO body. A resource not addressable by URL cannot be cached, rate-limited, or targeted by API gateways.
- Sub-resource routes: `/resource/{id}/sub-resource/{subId}`.
- Do not repeat the controller name inside `[Http*]` route templates when using `[Route("api/[controller]")]` — this doubles the segment (e.g., `[HttpPost("recipes/{id}/ingredients")]` on `RecipesController` resolves to `/api/recipes/recipes/{id}/ingredients`).

### Sharing surface
Every shareable resource requires three endpoints: grant (POST), revoke (DELETE), and shared-with-me (GET). A share endpoint without a revoke endpoint makes sharing permanent through the API.

### Auth-failure type consistency
The `Result<T>` type in an early-return auth failure must match the type the action returns on the success path. A mismatch (e.g., `Result<ResourcePermissionDto>` in a method that succeeds with `Result<IEnumerable<MealDto>>`) produces a malformed failure response body.
