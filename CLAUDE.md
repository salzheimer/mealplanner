# Meal Planning Microservices

.NET 10 microservices system for meal planning. Services communicate over HTTP via Docker Compose DNS. JWT authentication is issued by IdentityService and validated by downstream services.

---

## Essential Commands

### Run the full stack
```bash
# From repo root — create the external volume once if it doesn't exist
docker volume create mealplanningdb_data

# Start all services
docker compose -f mealplanner/infrastructure/docker/docker-compose.yml up --build
```

### Build a single service locally
```bash
dotnet build mealplanner/services/PlanService/src/PlanService.csproj
dotnet build mealplanner/services/MealService/src/MealService.csproj
dotnet build mealplanner/services/IdentityService/src/IdentityService.csproj
dotnet build mealplanner/services/ApiGateway/src/ApiGateway.csproj
```

### Run a single service locally
```bash
cd mealplanner/services/PlanService/src && dotnet run
```

### View logs
```bash
docker compose -f mealplanner/infrastructure/docker/docker-compose.yml logs -f <service-name>
# service names: postgres, auth-service, meal-service, plan-service, api-gateway
```

---

## Service Ports (host → container)

| Service      | Host Port | Scalar UI                          |
|--------------|-----------|------------------------------------|
| api-gateway  | 5001      | http://localhost:5001/scalar/v1    |
| auth-service | 5002      | http://localhost:5002/scalar/v1    |
| meal-service | 5003      | http://localhost:5003/scalar/v1    |
| plan-service | 5004      | http://localhost:5004/scalar/v1    |
| postgres     | 5432      | —                                  |

OpenAPI JSON: `http://localhost:<port>/openapi/v1.json`
Health check: `http://localhost:<port>/health`

---

## Project Structure

```
mealplanner/
├── CLAUDE.md                        # This file
├── docs/
│   ├── meal_planning_datamodel.drawio
│   ├── architecture.drawio
│   └── adr/                         # Architectural Decision Records
├── infrastructure/
│   ├── docker/
│   │   └── docker-compose.yml       # Orchestration for all services + postgres
│   └── postgres/
│       └── init.sql                 # Authoritative DB schema
├── services/
│   ├── ApiGateway/src/              # Routes requests, no DB connection
│   ├── IdentityService/src/             # JWT issuance, user registration/login
│   ├── MealService/src/             # Recipes, ingredients, meal scheduling
│   └── PlanService/src/             # Meal plans and meal item tracking
└── shared/
    ├── Shared.Models/               # DTOs, enums, JwtSettings record
    ├── Shared.Services/             # TokenService, UserService
    └── Shared.Tests/
```

---

## Architecture

- **IdentityService** issues JWT tokens. All other services validate JWTs using Bearer auth middleware configured with the same `JwtSettings`.
- **ApiGateway** proxies to downstream services. It does not connect to the database.
- **MealService** and **PlanService** connect to postgres via `ConnectionStrings__Postgres` (injected via docker-compose environment).
- Services discover each other by Docker Compose service name (e.g. `http://meal-service/api/recipes`).
- All services expose `GET /health` returning `{ "status": "Healthy" }`.

---

## Database

- **Engine:** PostgreSQL 17
- **Database:** `mealplannerdb`
- **User:** `mealplanner_user`
- **Schema source of truth:** `infrastructure/postgres/init.sql` — edit this file to change the schema, not individual services.
- The volume `mealplanningdb_data` is external — create it manually before first run (see Essential Commands above). `init.sql` only runs on first initialization of a fresh volume.

---

## Shared Models (`Shared.Models` namespace)

Enums mirror the postgres enum types exactly:
- `Visibility`: Private, Shared, Group
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
