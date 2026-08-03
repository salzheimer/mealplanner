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

## Verification

1. `docker compose up` — confirm `GET /api/auth/health`-equivalent routes proxy correctly through `api-gateway` to each service by DNS name.
2. Confirm frontend's existing relative `/api/*` calls work unchanged against the now-functional gateway (no frontend code changes required).
3. Query `/graphql` for the dashboard case (meal plans + meals in one round trip) and confirm it matches today's two-hook client-side join.
4. Confirm JWT validation still happens correctly at each downstream service (unchanged from today) when routed through YARP.
