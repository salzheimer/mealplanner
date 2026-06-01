Find and read every *Controller.cs file under mealplanner/services/ that is NOT in a tests/ or obj/ directory:

```bash
find mealplanner/services -name "*Controller.cs" -not -path "*/tests/*" -not -path "*/obj/*"
```

Read each file completely. Then produce a structured assessment across the four dimensions below. If prior assessment output exists in context, open with a **"What changed"** section listing fixes before listing new findings.

---

## SECURITY — check each controller for:

- Any action method returning `Result<T>` directly instead of `IActionResult`
  (failed operations respond with HTTP 200; clients cannot detect failure by status code)
- Endpoints missing `[Authorize]` that should be protected
- Endpoints exposed without auth that reveal system capabilities or accept sensitive input
  (e.g., unauthenticated `validatePassword` accepting password as a query string → logs)
- Resource ownership not verified: authenticated user ID extracted but never compared to the
  resource being accessed/modified (any authenticated user can mutate another user's data)
- IDOR: user ID accepted from query string/body without comparing to authenticated user
- Enum parsing using `throw` instead of returning a 400 result
- Null-forgiving operator (`!`) on values that could legitimately be null
- Wrong `Result<T>` type in auth-failure early return
  (e.g., `HandleResult(Result<ResourcePermissionDto>.Failure(...))` when success path
  returns `Result<IEnumerable<MealDto>>` — failure body has wrong shape)
- Wrong error namespace for the service:
  - `PlansController` → `PlanningErrors`
  - `MealPlanController` → `MealPlanErrors`
  - `MealController` → `MealErrors`
  - `RecipesController` → `RecipeErrors`
  - `AuthController` / `PermissionController` → `UserErrors` / `PermissionErrors`

---

## AMBIGUITY — check each controller for:

- Verbs in URL segments: `create-`, `add-`, `update-`, `delete-`, `grant-`, `validate-`
- PUT or DELETE endpoints where the resource ID is only in the DTO body, not the route
  (resource is not addressable; REST clients, gateways, caches cannot operate on it by URL)
- Route composition doubling the controller prefix
  (e.g., `[Route("api/[controller]")]` + `[HttpPost("recipes/{id}/ingredients")]`
  resolves to `/api/recipes/recipes/{id}/ingredients`)
- Naming inconsistency across services for the same concept
  (e.g., `serve-date` in MealPlanController vs `start-date` in PlansController,
  both calling the same underlying method)
- Singular/plural inconsistency within a resource's sub-routes
- Redundant `[Authorize]` on action methods when the controller class already has `[Authorize]`
- `[FromBody]` missing on complex-type parameters in PUT/POST actions
- ID type mismatch across the API (e.g., `permissionId:long` when all others are `int`)
- Redundant double-check pattern: manually checking `!result.IsSuccess` before calling
  `HandleResult`, which already handles both branches

---

## COMPLETENESS — for each resource, verify the full CRUD surface:

**Top-level resources** (meals, recipes, plans, mealplans):
- `GET /resource` — list all for authenticated user
- `GET /resource/{id}` — get one
- `POST /resource` — create (not `POST /resource/create-resource`)
- `PUT /resource/{id}` — update with ID in route
- `DELETE /resource/{id}` — delete with ID in route

**Sub-resources** (ingredients, instructions, meal items, plan items):
- `GET /resource/{id}/sub` — list
- `POST /resource/{id}/sub` — add (not `POST /resource/{id}/add-sub`)
- `PUT /resource/{id}/sub/{subId}` — update with BOTH IDs in route
- `DELETE /resource/{id}/sub/{subId}` — delete with BOTH IDs in route

**Sharing surface** (for each shareable resource):
- `POST /resource/{id}/share` — grant access
- `DELETE /permissions/{permissionId}` or `DELETE /resource/{id}/share/{shareId}` — revoke
- `GET /resource/shared-with-me` — inbound discovery for recipients

**Identity/auth surface:**
- `GET /api/auth/me` — authenticated user's own profile
- `PUT /api/auth/user` — update profile (display name, email)
- Password reset / forgot-password flow
- `GET /api/auth/sessions` — list and revoke active sessions

---

## MISSING FUNCTIONALITY — meal planning application gaps:

- Can a user browse their meal library? (`GET /api/meal`)
- Can a user list all items in a meal? (`GET /api/meal/{id}/items`)
  (items support add/update/delete but not list; `GET /{id}/recipes` silently omits
  non-Recipe item types: Homemade, StoreBought)
- Does `GET /api/meal/{mealId}/recipes` return 200 with a partial list when some recipes
  fail to load? (caller cannot detect missing data)
- Can a user navigate from a plan to its meal plans? (`GET /api/plans/{id}/mealplans`)
- Does `GrantPermission` verify the caller owns the resource being shared?
  (any authenticated user can grant others access to any resource they don't own)
- Are there unused imports? (`using System.Reflection.Metadata`,
  `using System.Security.Claims` when claims are in BaseController)

---

## OUTPUT FORMAT

1. **What changed** (only if prior assessment is visible in context)
2. **Findings by service** — group under the service name, then Security / Ambiguity /
   Completeness headings. Label each: `[CRITICAL]` `[HIGH]` `[MEDIUM]` `[LOW]`
3. **Priority table** at the end:

| Priority | Service | Issue |
|---|---|---|
| 1 | ... | ... |
