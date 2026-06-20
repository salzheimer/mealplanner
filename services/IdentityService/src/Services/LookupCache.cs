using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services;

public interface ILookupCache
{
    int GetClientTypeId(string name);
    int GetGroupMemberRoleId(string name);
    int GetGroupMemberStatusId(string name);
    Task RefreshAsync();
}

public class LookupCache : ILookupCache
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ReaderWriterLockSlim _lock = new();

    private Dictionary<string, int> _clientTypes = new();
    private Dictionary<string, int> _groupMemberRoles = new();
    private Dictionary<string, int> _groupMemberStatuses = new();

    public LookupCache(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public int GetClientTypeId(string name)
    {
        _lock.EnterReadLock();
        try { return _clientTypes[name]; }
        finally { _lock.ExitReadLock(); }
    }

    public int GetGroupMemberRoleId(string name)
    {
        _lock.EnterReadLock();
        try { return _groupMemberRoles[name]; }
        finally { _lock.ExitReadLock(); }
    }

    public int GetGroupMemberStatusId(string name)
    {
        _lock.EnterReadLock();
        try { return _groupMemberStatuses[name]; }
        finally { _lock.ExitReadLock(); }
    }

    public async Task RefreshAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserContext>();

        var clientTypes = await context.ClientType
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        var groupMemberRoles = await context.GroupMemberRoles
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        var groupMemberStatuses = await context.GroupMemberStatuses
            .ToDictionaryAsync(x => x.Name, x => x.Id);

        _lock.EnterWriteLock();
        try
        {
            _clientTypes = clientTypes;
            _groupMemberRoles = groupMemberRoles;
            _groupMemberStatuses = groupMemberStatuses;
        }
        finally { _lock.ExitWriteLock(); }
    }
}

public class LookupCacheWarmup : IHostedService
{
    private readonly ILookupCache _cache;
    private readonly ILogger<LookupCacheWarmup> _logger;

    public LookupCacheWarmup(ILookupCache cache, ILogger<LookupCacheWarmup> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _cache.RefreshAsync();
        _logger.LogInformation("Lookup cache loaded.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
