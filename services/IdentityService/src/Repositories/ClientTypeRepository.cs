
using Microsoft.EntityFrameworkCore;
using IdentityService.Models;
using IdentityService.Services;

namespace IdentityService.Repositories;

public partial class ClientTypeRepository : Interfaces.IClientTypeRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILookupCache _lookupCache;

    public ClientTypeRepository(IdentityDbContext context, ILookupCache lookupCache)
    {
        _context = context;
        _lookupCache = lookupCache;
    }

    public async Task<List<ClientTypes>> GetAllClientTypes()
    {
        return await _context.ClientType.ToListAsync();
    }

    public async Task<ClientTypes?> GetClientTypeById(int id)
    {
        return await _context.ClientType.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<int> CreateClientType(ClientTypes clientType)
    {
        _context.ClientType.Add(clientType);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }

    public async Task<int> UpdateClientType(ClientTypes clientType)
    {
        var existing = await _context.ClientType.FirstOrDefaultAsync(o => o.Id == clientType.Id);
        if (existing == null) return 0;

        existing.Name = clientType.Name;
        existing.DisplayName = clientType.DisplayName;
        existing.SortOrder = clientType.SortOrder;

        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }

    public async Task<int> DeleteClientType(int id)
    {
        var existing = await _context.ClientType.FirstOrDefaultAsync(o => o.Id == id);
        if (existing == null) return 0;

        _context.ClientType.Remove(existing);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }
}