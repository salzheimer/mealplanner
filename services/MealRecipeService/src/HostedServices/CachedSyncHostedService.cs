using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Bus;
using Shared.Models;  
using MealRecipeService.Repositories;
using MealRecipeService.Interfaces; // Replace with your actual repo namespace

namespace MealRecipeService.HostedServices;

public class CacheSyncHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBus _bus;

    public CacheSyncHostedService(IServiceScopeFactory scopeFactory, IBus bus)
    {
        _scopeFactory = scopeFactory;
        _bus = bus;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 1. Tell RabbitMQ that this service wants to listen to these events
        // This creates the actual bindings between the Exchange and your Queue
        await _bus.Subscribe<UserChanged>();
        await _bus.Subscribe<GroupChanged>();
        await _bus.Subscribe<GroupMembershipChanged>();

        // 2. Safely check if the cache is empty in the background
        using var scope = _scopeFactory.CreateScope();
        var cacheService = scope.ServiceProvider.GetRequiredService<ICachedService>();
        
        if (await cacheService.IsIdentityCacheEmptyAsync())
        {
            
            // Requesting an initial snapshot sync from IdentityService
            await _bus.Send(new CacheSyncRequested { RequestedBy = "MealRecipeService" });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}