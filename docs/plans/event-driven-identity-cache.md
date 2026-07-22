# Event-Driven Identity Cache Pattern

## Context

MealRecipeService (and future PlanService) need access to user, group, and group membership data to resolve resource permissions locally — without making synchronous HTTP calls to IdentityService at runtime. The `cached_users`, `cached_groups`, and `cached_group_members` tables already exist in `meal_recipe_db`, and C# models exist, but the repositories are read-only stubs and no sync mechanism is implemented.

The goal is to have IdentityService publish events when identity data changes, and have consumer services update their local caches. At runtime, AccessService reads only from the local DB — no cross-service calls for permission checks.

---

## Current State

- `CachedUser`, `CachedGroup`, `CachedGroupMember` models exist in `MealRecipeService/src/Models/Cached.cs`
- `CachedRepository.cs` exists but Create/Update/Delete throw `NotImplementedException`
- `IdentityService.csproj` and `MealRecipeService.csproj` already reference `MassTransit`/`MassTransit.RabbitMQ` **9.1.2** — MassTransit went commercial starting with v9, so these references need to be **removed**, not just left unused. `IdentityService/src/Program.cs` already calls `AddMassTransit(...)`, and both `appsettings.json` files already have a `RabbitMq` config section — this plan replaces that wiring with Rebus rather than adding messaging from scratch
- No `rabbitmq` service/volume in `docker-compose.yml` yet (Step 1 genuinely not started)
- No events published anywhere in IdentityService
- IdentityService has no C# models or repositories for `groups` or `group_members` (only lookup repos for roles/statuses)
- Docker-compose has no message broker infrastructure

---

## Implementation Plan

### Step 1: Add RabbitMQ to Infrastructure

Add RabbitMQ to `mealplanner/infrastructure/docker/docker-compose.yml`:

```yaml
rabbitmq:
  image: rabbitmq:3-management
  ports:
    - "5672:5672"
    - "15672:15672"
  environment:
    RABBITMQ_DEFAULT_USER: mealplanner
    RABBITMQ_DEFAULT_PASS: mealplanner
  volumes:
    - rabbitmq_data:/var/lib/rabbitmq
```

Add `rabbitmq_data` to the volumes section and add `depends_on: rabbitmq` to both `identity-service` and `meal-recipe-service`.

Add RabbitMQ connection string to `appsettings.json` in each service:
```json
"RabbitMq": {
  "Host": "rabbitmq",
  "Username": "mealplanner",
  "Password": "mealplanner"
}
```

---

### Step 2: Create Shared Event Contracts

Add event message types to `mealplanner/shared/Shared.Models/`. These are the contracts both publisher and consumer depend on.

```csharp
// IdentityEvents.cs
public record UserChanged
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;  // Created, Updated, Deleted
}

public record GroupChanged
{
    public Guid GroupId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;  // Created, Updated, Deleted
}

public record GroupMembershipChanged
{
    public Guid UserId { get; init; }
    public Guid GroupId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public string StatusName { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;  // Added, Updated, Removed
}

public record CacheSyncRequested
{
    public string RequestedBy { get; init; } = string.Empty;  // triggers IdentityService to re-publish all current state
}
```

Use init-only properties rather than positional (primary constructor) form. Rebus's default JSON serialization works by property name — init-only records allow adding optional properties in future without breaking existing consumers. Carry the full payload so consumers never need to call back to IdentityService on receipt.

---

### Step 3: Add Group and GroupMember Models to IdentityService

IdentityService has `groups` and `group_members` tables in its DB but no C# domain models. Add:

- `Group` and `GroupMember` models in `IdentityService/src/Models/`
- `IGroupRepository` / `GroupRepository` in `IdentityService/src/Repositories/`
- `IGroupMemberRepository` / `GroupMemberRepository`
- Register both in `IdentityService/src/Program.cs`

Map `GroupRepository` to the `groups` table and `GroupMemberRepository` to `group_members`.

---

### Step 4: Remove MassTransit, Add Rebus + Publishing to IdentityService

**Remove first:** `IdentityService.csproj` currently references `MassTransit`/`MassTransit.RabbitMQ` **9.1.2** (the commercial version) and `Program.cs` already calls `AddMassTransit(...)`. Remove both the package references and that block before adding Rebus — don't leave the commercial packages installed alongside the new ones.

**Packages (all MIT-licensed, free):** `Rebus`, `Rebus.RabbitMq`, `Rebus.ServiceProvider`

Configure Rebus in `IdentityService/src/Program.cs`, reusing the existing `RabbitMq:*` config keys already in `appsettings.json`:

```csharp
builder.Services.AddRebus(configure => configure
    .Logging(l => l.Console())
    .Transport(t => t.UseRabbitMq(
        $"amqp://{builder.Configuration["RabbitMq:Username"]}:{builder.Configuration["RabbitMq:Password"]}@{builder.Configuration["RabbitMq:Host"]}",
        "identity-service")));
```

Inject Rebus's `IBus` (not MassTransit's `IPublishEndpoint`) into `UserService` and publish after each mutating operation:

- `CreateUserAsync` → publish `UserChanged(..., "Created")`
- Any update path → publish `UserChanged(..., "Updated")`
- Any delete path → publish `UserChanged(..., "Deleted")`

Add a `GroupService` (new) that wraps `GroupRepository` and `GroupMemberRepository`, publishing:

- Group create/update/delete → `GroupChanged`
- Group member add/remove → `GroupMembershipChanged`

---

### Step 5: Implement Cache Write Operations in MealRecipeService

Enable write operations in `MealRecipeService/src/Repositories/CachedRepository.cs`. Currently Create/Update/Delete throw `NotImplementedException`. Implement upsert logic for:

- `CachedUserRepository`: upsert by `user_id`, set `display_name` and `synced_at`
- `CachedGroupRepository`: upsert by `group_id`, set `group_name` and `synced_at`
- `CachedGroupMemberRepository` (new or extend existing): upsert/delete by `(user_id, group_id)`

Use EF Core's `AddOrUpdate` pattern or raw SQL upsert (`INSERT ... ON CONFLICT DO UPDATE`).

---

### Step 6: Remove MassTransit, Add Rebus Handlers to MealRecipeService

**Remove first:** `MealRecipeService.csproj` currently references `MassTransit`/`MassTransit.RabbitMQ` **9.1.2** even though `Program.cs` never calls `AddMassTransit` yet. Remove the package references.

**Packages (all MIT-licensed, free):** `Rebus`, `Rebus.RabbitMq`, `Rebus.ServiceProvider`

Create handlers in `MealRecipeService/src/Consumers/` (Rebus's handler interface is `IHandleMessages<T>`, not MassTransit's `IConsumer<T>`):

```csharp
public class UserChangedHandler : IHandleMessages<UserChanged>
{
    public async Task Handle(UserChanged msg)
    {
        if (msg.Action == "Deleted")
            await _cachedUserRepository.DeleteAsync(msg.UserId);
        else
            await _cachedUserRepository.UpsertAsync(msg.UserId, msg.DisplayName);
    }
}
```

Add `GroupChangedHandler` and `GroupMembershipChangedHandler` following the same pattern.

Configure Rebus in `MealRecipeService/src/Program.cs` with its own input queue, then auto-register handlers:

```csharp
builder.Services.AddRebus(configure => configure
    .Logging(l => l.Console())
    .Transport(t => t.UseRabbitMq(
        $"amqp://{builder.Configuration["RabbitMq:Username"]}:{builder.Configuration["RabbitMq:Password"]}@{builder.Configuration["RabbitMq:Host"]}",
        "meal-recipe-service")));
builder.Services.AutoRegisterHandlersFromAssemblyOf<Program>();
```

Rebus creates durable RabbitMQ queues automatically, so missed events while the service is down are still replayed on reconnect — same guarantee MassTransit provided.

**Important difference from MassTransit:** Rebus requires each subscriber to explicitly call `bus.Subscribe<TEvent>()` once at startup so RabbitMQ knows to route published events to that queue. MassTransit infers this topology automatically from registered consumers; Rebus does not. Add this to the startup hosted service in Step 7:

```csharp
await bus.Subscribe<UserChanged>();
await bus.Subscribe<GroupChanged>();
await bus.Subscribe<GroupMembershipChanged>();
```

Forgetting this means events silently never arrive at MealRecipeService — there's no error, the queue just never receives anything.

---

### Step 7: Initial Cache Population

The cache starts empty on first deployment and any time a consumer service's DB is wiped. The rebuild mechanism uses the message bus so no admin endpoints are required and the same consumer code handles both initial load and ongoing updates.

**Approach — SyncRequested event:**

1. MealRecipeService registers an `IHostedService` that runs at startup. If `cached_users` is empty, it publishes a `CacheSyncRequested` event to the bus.
2. IdentityService registers a `CacheSyncRequestedConsumer` that handles the event by re-publishing all current users, groups, and memberships as their normal change events (`UserChanged`, `GroupChanged`, `GroupMembershipChanged` with `Action = "Created"`).
3. MealRecipeService's existing consumers receive those events and populate the cache tables normally.

```csharp
// MealRecipeService — startup hosted service
public class CacheSyncHostedService(IServiceScopeFactory scopeFactory, IBus bus) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await bus.Subscribe<UserChanged>();
        await bus.Subscribe<GroupChanged>();
        await bus.Subscribe<GroupMembershipChanged>();

        using var scope = scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICachedUserRepository>();
        if (!await repo.AnyAsync())
            await bus.Publish(new CacheSyncRequested { RequestedBy = "MealRecipeService" });
    }
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

```csharp
// IdentityService — sync request handler
public class CacheSyncRequestedHandler(IUserRepository users, IGroupRepository groups,
    IGroupMemberRepository members, IBus bus) : IHandleMessages<CacheSyncRequested>
{
    public async Task Handle(CacheSyncRequested message)
    {
        foreach (var user in await users.GetAllAsync())
            await bus.Publish(new UserChanged { UserId = user.Id, DisplayName = user.DisplayName, Action = "Created" });
        foreach (var group in await groups.GetAllAsync())
            await bus.Publish(new GroupChanged { GroupId = group.Id, GroupName = group.Name, Action = "Created" });
        foreach (var member in await members.GetAllAsync())
            await bus.Publish(new GroupMembershipChanged { UserId = member.UserId, GroupId = member.GroupId, RoleName = member.RoleName, Action = "Added" });
    }
}
```

This mechanism also serves as the cache rebuild path if the cache ever becomes stale — publish a `CacheSyncRequested` event manually via the RabbitMQ management UI (`http://localhost:15672`) without any code changes or deployments.

---

## Files to Create or Modify

| File | Change |
|------|--------|
| `infrastructure/docker/docker-compose.yml` | Add RabbitMQ service and volume |
| `shared/Shared.Models/IdentityEvents.cs` | New — event contracts |
| `IdentityService/src/Models/Group.cs` | New — Group, GroupMember models |
| `IdentityService/src/Repositories/GroupRepository.cs` | New |
| `IdentityService/src/Services/GroupService.cs` | New — includes event publishing |
| `IdentityService/src/Services/UserService.cs` | Add event publishing on mutating ops |
| `IdentityService/src/Program.cs` | Remove `AddMassTransit`; register Rebus, GroupRepository, GroupService |
| `IdentityService/IdentityService.csproj` | Remove MassTransit/MassTransit.RabbitMQ 9.1.2; add Rebus, Rebus.RabbitMq, Rebus.ServiceProvider |
| `MealRecipeService/src/Repositories/CachedRepository.cs` | Implement write operations |
| `MealRecipeService/src/Consumers/UserChangedHandler.cs` | New |
| `MealRecipeService/src/Consumers/GroupChangedHandler.cs` | New |
| `MealRecipeService/src/Consumers/GroupMembershipChangedHandler.cs` | New |
| `MealRecipeService/src/Program.cs` | Register Rebus, auto-register handlers |
| `MealRecipeService/MealRecipeService.csproj` | Remove MassTransit/MassTransit.RabbitMQ 9.1.2; add Rebus, Rebus.RabbitMq, Rebus.ServiceProvider |
| `MealRecipeService/src/HostedServices/CacheSyncHostedService.cs` | New — subscribes to events, publishes CacheSyncRequested on empty cache |
| `IdentityService/src/Consumers/CacheSyncRequestedHandler.cs` | New — re-publishes all current state as change events |

---

## Verification

1. Start the stack with `docker-compose up` — RabbitMQ management UI should be accessible at `http://localhost:15672`
2. Create a user via IdentityService — verify a `cached_users` row appears in `meal_recipe_db`
3. Add a user to a group — verify a `cached_group_members` row appears
4. Stop MealRecipeService, create another user, restart MealRecipeService — verify the missed event is replayed and the cache is updated (Rebus durable RabbitMQ queue)
5. Wipe `cached_users` directly in the DB, restart MealRecipeService — verify `CacheSyncRequested` is published on startup and the cache repopulates via events
6. Publish a `CacheSyncRequested` event manually via the RabbitMQ management UI — verify the cache rebuilds without a service restart. Note: Rebus wraps messages with its own headers (e.g. `rbs2-msg-type`) for deserialization, so a hand-typed message via the management UI must set those headers correctly to be picked up — otherwise prefer triggering this via a small test script/endpoint instead of the raw UI
