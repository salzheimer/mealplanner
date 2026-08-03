# API Gateway: Local-Free-Now, Azure APIM-Later Plan

## Context

`ApiGateway` (`services/ApiGateway/src/Program.cs`) is currently a stub — health checks, controllers, OpenAPI, no actual routing (`GatewayController.cs` exposes only `GET api/gateway/status`). The frontend already calls relative `/api/*` paths (`frontend/src/services/apiClient.ts`) assuming a working single-origin gateway exists.

Two things are now known that weren't part of the original plan:
1. Azure APIM is the intended production gateway, but not yet — deploy timing is undecided.
2. Cross-service aggregate views are expected to grow beyond the dashboard (`features/dashboard/DashboardPage.tsx` already calls `useMealPlansByDateRange` (PlanService) and `useMeals` (MealRecipeService) separately and joins client-side — the first instance of this need).

Goal: get a working, **zero-cost** gateway for local development now, without doing work that gets thrown away when APIM is introduced.

---

## Core idea: separate the "dumb proxy" from the "smart aggregator"

These have different lifecycles and should not be the same piece of code:

| Concern | Role | Lifecycle |
|---|---|---|
| Path routing, TLS, per-route auth/rate-limit policy | **Dumb proxy** — pure config, no business logic | Free now via YARP; this exact job gets handed to Azure APIM later. Because it's config-only, migrating it is mechanical, not a rewrite. |
| Cross-service composite queries (dashboard today, more later) | **Smart aggregator** — actual code that composes calls to multiple services | Has to live in your own code regardless of which gateway fronts it. APIM cannot do this. Build it once, gateway-agnostic, and it never needs to move. |

Keeping these separate means only the proxy layer is disposable/replaceable — the aggregation layer is a permanent asset either way.

---

## Phase 1 — Local development, $0 (now)

### 1. Finish `ApiGateway` as a YARP reverse proxy
Add `Yarp.ReverseProxy` (MIT-licensed, free) to `services/ApiGateway/src/ApiGateway.csproj`. Configure routes/clusters in `appsettings.json`, targeting the existing Docker Compose service DNS names (`auth-service`, `meal-service`, `plan-service` — per `CLAUDE.md`):

```json
"ReverseProxy": {
  "Routes": {
    "identity": { "ClusterId": "identity", "Match": { "Path": "/api/auth/{**catch-all}" } },
    "meal":     { "ClusterId": "meal",     "Match": { "Path": "/api/{recipes|meal}/{**catch-all}" } },
    "plan":     { "ClusterId": "plan",     "Match": { "Path": "/api/plan/{**catch-all}" } }
  },
  "Clusters": {
    "identity": { "Destinations": { "d1": { "Address": "http://auth-service" } } },
    "meal":     { "Destinations": { "d1": { "Address": "http://meal-service" } } },
    "plan":     { "Destinations": { "d1": { "Address": "http://plan-service" } } }
  }
}
```

`Program.cs` adds only:
```csharp
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
// ...
app.MapReverseProxy();
```

Document this path→service mapping in exactly one place (this file, or a new table in `CLAUDE.md`) — it's the single source of truth that both the YARP config today and the APIM import later will mirror.

### 2. Build the GraphQL BFF as its own project, not inside the YARP host
New project, e.g. `services/GatewayBff` (HotChocolate — free, open source), exposing `/graphql`. Resolvers call the existing REST APIs (identity/meal/plan) the same way the frontend does today — no new business logic, just composition. Start with the one proven case (dashboard: meal plans + meals in one query) and add composite queries as new cross-service views show up.

Keep this as a **separate deployable** from `ApiGateway`/YARP. That separation is what makes it gateway-agnostic — YARP routes `/graphql` to it today, APIM will route `/graphql` to it later, identically.

### 3. What to route where
- YARP (`ApiGateway`): `/api/*` → pass straight through to the owning service, unchanged from today's per-service REST contracts.
- YARP: `/graphql` → `GatewayBff`.
- JWT validation: **leave it exactly where it is today** (per-service Bearer auth, per `CLAUDE.md`). Do not centralize auth into YARP now — that's specifically the job APIM's `validate-jwt` policy will take over in Phase 2, and building it twice is wasted effort.
- Rate limiting / response caching: **skip for now.** Nothing in the current scale needs it, and it's a built-in APIM policy later — adding custom middleware here is exactly the kind of throwaway work this plan is trying to avoid.

### Docs/exploration surfaces are unaffected by the proxy
Each service maps its own Scalar UI and `/openapi/v1.json` directly in its own `Program.cs`, served on its own host port (5002/5003/5004 per `CLAUDE.md`). YARP in `ApiGateway` only intercepts traffic hitting the gateway's own port (5001) under `/api/*` and `/graphql` — it does not proxy or remove each service's directly-exposed Scalar endpoints, so `http://localhost:500{2,3,4}/scalar/v1` keep working exactly as they do today, unchanged by this plan.

Scalar is OpenAPI/REST-specific and will not render the new `/graphql` schema. HotChocolate ships its own free GraphQL IDE (Banana Cake Pop) at the BFF's own endpoint for that purpose — a separate explorer, not a Scalar view, mirroring how each REST service already has its own independent docs surface.

---

## Phase 2 — Azure APIM arrives (later, cost incurred then)

1. Each service already exposes OpenAPI JSON at `/openapi/v1.json` (per `CLAUDE.md`) — import each directly into APIM as a separate API/product. This turns the path-mapping table from Phase 1 into APIM routing config with minimal manual re-entry, because the mapping was already documented once.
2. Move JWT validation to APIM's `validate-jwt` policy (validated against IdentityService's signing config). Move rate limiting to APIM policies (`rate-limit-by-key`, etc.) if/when actually needed.
3. `GatewayBff` (`/graphql`) gets imported into APIM as a single backend/route, same as any REST API — no code changes, because it was built gateway-agnostic in Phase 1.
4. `ApiGateway`/YARP is **not deleted**. It keeps running as the local `docker compose up` proxy so the dev loop stays free and cloud-independent (this also matches the standing "Docker deployment required" preference) — it simply stops being the production-facing edge once APIM takes that role. Nothing built in Phase 1 becomes wasted work; the proxy config was always meant to be disposable at the production edge, not in local dev.

---

## What NOT to build in Phase 1 (avoid throwaway work)

- No custom rate-limiting middleware — it's an APIM policy later.
- No centralized JWT validation in YARP — APIM's job later; touching auth twice doubles the risk of introducing a bug for no interim benefit.
- No attempt to replicate APIM-only features locally (developer portal, subscription keys, products, versioning) — not needed for the dev loop.

---

## Files to Create or Modify

| File | Change |
|---|---|
| `services/ApiGateway/src/ApiGateway.csproj` | Add `Yarp.ReverseProxy` package reference |
| `services/ApiGateway/src/Program.cs` | Add `AddReverseProxy()` / `MapReverseProxy()`; remove or repurpose stub `GatewayController` |
| `services/ApiGateway/src/appsettings.json` | Add `ReverseProxy` routes/clusters section |
| `services/GatewayBff/` | New project — HotChocolate GraphQL server, resolvers call existing REST APIs |
| `infrastructure/docker/docker-compose.yml` | Add `gateway-bff` service; ensure `api-gateway` routes `/graphql` to it |
| `CLAUDE.md` or this file | Path → service ownership table (single source of truth for YARP now, APIM later) |

---

## Implementation Checklist

**Status: Phase 1 implemented.** See "Implementation Notes" below for where reality diverged from the plan as written above.

### 0. Housekeeping (do this first)
- [x] Investigated `services/ApiGateway/tests/ApiGateway.Tests/bin/Debug/net10.0` — confirmed leftover from a removed package reference (no source referenced Yarp/HotChocolate). Deleted all `bin`/`obj` across the solution and rebuilt clean before starting.

### 1. YARP reverse proxy (`ApiGateway`)
- [x] Added `Yarp.ReverseProxy` package reference to `services/ApiGateway/src/ApiGateway.csproj`
- [x] Added a `ReverseProxy` routes/clusters section to `services/ApiGateway/src/appsettings.json` (+ `appsettings.Development.json` override for running outside Docker)
- [x] Wired `AddReverseProxy().LoadFromConfig(...)` and `MapReverseProxy()` into `Program.cs`
- [x] Kept the stub `GatewayController` (`GET api/gateway/status`) as-is — no route conflicts with the new proxy paths
- [x] Cluster destinations use the **real** Docker Compose service names — see Implementation Notes (they are not `auth-service`/`meal-service`/`plan-service`)
- [x] **Tests** (`services/ApiGateway/tests/ApiGateway.Tests/ReverseProxyRoutingTests.cs`, `WebApplicationFactory<Program>`):
  - [x] Route-match tests, implemented by pointing every cluster at an unreachable address and asserting **502** (route matched, proxy attempted) vs **404** (no route matched) — verifies real routing behavior without needing live backend services
  - [x] Unmatched-path test
  - [x] `GatewayControllerTests.cs` unchanged; added a routing test confirming `/api/gateway/status` isn't shadowed by the proxy

### 2. GraphQL BFF (new `GatewayBff` project)
- [x] Scaffolded `services/GatewayBff/src/GatewayBff.csproj`, added to `meal_planning_monorepo.sln`
- [x] Added `HotChocolate.AspNetCore` (v16.5.1)
- [x] `Query.GetDashboardMealPlans` (GraphQL field `dashboardMealPlans`) composes PlanService's date-range meal plans with MealRecipeService's meal list
- [x] Resolver forwards the caller's JWT (via `IHttpContextAccessor`) into both downstream calls
- [x] `MapGraphQL().WithOptions(o => o.Tool.Enable = app.Environment.IsDevelopment())` gates Banana Cake Pop to Development
- [x] Added `services/GatewayBff/tests/GatewayBff.Tests`, added to the `.sln`
- [x] **Tests** (13 total — `DashboardServiceTests.cs`, `QueryTests.cs`, `GraphQLIntegrationTests.cs`):
  - [x] Resolver/service unit tests with mocked HTTP responses
  - [x] End-to-end `WebApplicationFactory` test posting a real GraphQL query
  - [x] Auth-forwarding test — missing/invalid bearer token → GraphQL error, resolver never invoked (verified via mock)
  - [x] Partial-failure test — MealRecipeService failing still returns meal plans (with `meal: null`) plus a field-level error, rather than failing the whole query

### 3. Docker Compose wiring
- [x] Added `gateway_bff` service (port `5005:80`, `depends_on` `meal_recipe_service` + `plan_service`)
- [x] Added `gateway_bff` to `api_gateway`'s `depends_on`
- [x] `/graphql` (exact + catch-all, for the IDE/websocket paths) routed to the `bff` cluster in YARP config

### 4. Docs
- [x] Added the path → service ownership table to `CLAUDE.md` ("Request Routing" under Architecture)
- [x] Added `gateway_bff`'s port to the Service Ports table in `CLAUDE.md`
- [x] Updated `CLAUDE.md`'s Project Structure, Essential Commands, and Database sections — these were stale independent of this plan (see notes) and the "Run the full stack" instructions were actually broken

---

## Implementation Notes (where reality diverged from the plan)

- **The routing gap didn't actually exist — it was just implemented as nginx, not YARP.** `frontend/nginx.conf` already had per-service `proxy_pass` rules (and `docker-compose.yml` had **no `api_gateway` service at all** — `ApiGateway` wasn't part of the running stack). Asked the user how to reconcile this; chose to replace nginx's routing with `ApiGateway`/YARP (nginx now forwards everything under `/api/` and `/graphql` to `api_gateway`) rather than leave two parallel routers, per the original APIM-migration reasoning.
- **`CLAUDE.md`'s service names/ports were stale.** Real Docker Compose service names are `identity_service`, `meal_recipe_service`, `plan_service` (not `auth-service`/`meal-service`/`plan-service`), and the database has already been split into `identity_db`/`meal_recipe_db`/`plan_db` with three external volumes — `CLAUDE.md` still described the pre-split single `mealplannerdb`/`mealplanningdb_data` state. Fixed as part of this work since the "Run the full stack" instructions were the ones being extended.
- **A required REST endpoint was missing.** `MealController` never exposed a "list all meals" route even though `IMealService.GetAllMealsAsync` already existed and the frontend's `mealService.list()` already called `GET /api/meal` expecting one. Added the missing `[HttpGet] GetAllMeals()` action (+ 2 tests) since the BFF's dashboard resolver depends on it — this was a pre-existing gap, not something introduced by the gateway work.
- **`ServiceClient.CreateClient` isn't unit-testable as-is** — it opens a real `HttpClient` with no seam for a mocked handler, and turned out to be unused anywhere in the actual codebase before this (only referenced in planning docs). Added `IServiceClientFactory`/`ServiceClientFactory` in `GatewayBff` as a thin wrapper so `DashboardService` can be tested with a mocked `HttpMessageHandler`; production behavior is unchanged (`ServiceClientFactory` just calls `ServiceClient.CreateClient`).
- **HotChocolate 16.x API differs slightly from older docs**: `MapGraphQL().WithOptions(...)` takes an `Action<GraphQLServerOptions>`, not an instance.
- Full solution build + test run: **0 errors, 182 tests passed** (Shared.Tests 6, PlanService.Tests 44, MealRecipeService.Tests 60 incl. new `GetAllMeals` tests, IdentityService.Tests 45, GatewayBff.Tests 13, ApiGateway.Tests 14).

---

## Verification

1. [x] `docker compose up` — `GET /api/auth/*`-style routes proxy correctly through `api_gateway` to each service by DNS name. **Live-verified** in a real Docker Compose stack (all 10 containers healthy).
2. [x] Frontend's existing relative `/api/*` calls work unchanged against the now-functional gateway (no frontend code changes required) — confirmed `http://localhost:3000/api/meal` proxies through nginx → `api_gateway` → `meal_recipe_service` correctly.
3. [x] Queried `/graphql` for the dashboard case (meal plans + meals in one round trip) with real data (registered a user, created a meal, a plan, and a meal plan through the gateway) — confirmed it returns the same composed shape `DashboardPage.tsx` currently builds from two client-side calls.
4. [x] JWT validation confirmed working end-to-end through YARP (register → login → authenticated calls to `meal_recipe_service` and `plan_service` via the gateway).
5. [x] Each service's own Scalar UI (`:5002`/`:5003`/`:5004` → `/scalar/v1`) confirmed unaffected (200s); Banana Cake Pop reachable at `:5005/graphql`.

### Bugs found and fixed during live verification (all pre-existing, none caused by this plan)

Live verification surfaced four unrelated, pre-existing bugs — each one blocked the next step of the check, so they were fixed in sequence (with sign-off) to complete it:

1. **`ASPNETCORE_ENVIRONMENT=Development` in Docker vs. `appsettings.Development.json`.** Docker Compose sets `Development` on every service (so Scalar works there too) — but that means `appsettings.Development.json` also loads *inside containers*, not just for local `dotnet run`. The Phase-1 plan above put `ApiGateway`/`GatewayBff`'s localhost cluster/service-URL overrides in `appsettings.Development.json`, which clobbered the correct Docker DNS addresses and made YARP proxy to `localhost:5003` etc. *inside the `api_gateway` container* (connection refused → 502 on every route). **Fix:** moved those overrides into `Properties/launchSettings.json`'s `environmentVariables` instead — that file is only read by `dotnet run`/IDE launch, never by the published container. Same fix applied to both `ApiGateway` and `GatewayBff`.
2. **JWT signing secret mismatch.** `services/MealRecipeService/src/appsettings.json` had a truncated `Jwt:Secret` (`"replace-this-with-a-secure-key"`) that didn't match `IdentityService`'s and `PlanService`'s secret (`"...-this-is-for-demo-use-only"`), so every JWT issued by IdentityService failed Bearer validation at MealRecipeService with 401 — reproduced by hitting `meal_recipe_service` directly, bypassing the gateway entirely. **Fix:** aligned the secret.
3. **EF Core table/column mappings didn't match the real schema in `MealRecipeService`.** Likely drifted during the db-per-service migration and invisible to the existing test suite (mocks the repository layer, never hits real Postgres). Fixed: `Meal` (`meal`→`meals`), `Recipe` (`recipe`→`recipes`), `MealItem` (`meal_item`→`meal_items`), `RecipeComponent` (`recipe_component`→`recipe_components`), `MealItemType` (table `meal_item_type`→`item_types`, PK column `meal_item_type_id`→`item_type_id`), `PermissionType` (PK column `permission_type_id`→`permission_id`), `ResourcePermission` (`granted_by_user_id`→`granted_by`, hit via `GetAllMealsAsync`'s shared-permission check). **Not fixed — flagged for a separate pass:** `RecipeIngredient` (PK column should likely be `ingredient_id`, not `recipe_ingredient_id`), `RecipeInstruction` (PK column should likely be `instruction_id`, not `recipe_instruction_id`), and `ResourcePermission.ResourceId` is `Guid` in C# but `integer` in Postgres — a type mismatch, not just a naming one, that needs actual investigation rather than a quick rename. None of these three block the meal/dashboard flow this plan cares about.
4. **`PlanService`'s date-range meal-plan queries silently excluded every meal plan without an explicit `EndDate`.** `MealPlanRepository.GetMealPlansByDateRangeAsync`/`GetMealPlansByEndDateAsync` filtered with `mp.EndDate <= endDate` — since `EndDate` is nullable (single-day meal plans, the common case, leave it `null`), `NULL <= endDate` is never true in SQL, so those rows were dropped. This is a real, user-facing bug in the *existing* dashboard feature (`useMealPlansByDateRange` on the frontend hits this exact endpoint today), not something introduced by this plan — this session's verification is what surfaced it. **Fix:** `(mp.EndDate ?? mp.ServeDate) <= endDate` in both methods.

All fixes verified with `dotnet test` (182/182 passing throughout) and confirmed live against the running Docker stack.
