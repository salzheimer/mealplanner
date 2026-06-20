using Microsoft.EntityFrameworkCore;
using IdentityService.Models;
using IdentityService.Services;

namespace IdentityService.Repositories;

public class GroupMemberStatusRepository : Interfaces.IGroupMemberStatusRepository
{
    private readonly UserContext _context;
    private readonly ILookupCache _lookupCache;

    public GroupMemberStatusRepository(UserContext context, ILookupCache lookupCache)
    {
        _context = context;
        _lookupCache = lookupCache;
    }

    public async Task<List<GroupMemberStatus>> GetAllAsync()
    {
        return await _context.GroupMemberStatuses.ToListAsync();
    }

    public async Task<GroupMemberStatus?> GetByIdAsync(int id)
    {
        return await _context.GroupMemberStatuses.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<int> CreateAsync(GroupMemberStatus status)
    {
        _context.GroupMemberStatuses.Add(status);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }

    public async Task<int> UpdateAsync(GroupMemberStatus status)
    {
        var existing = await _context.GroupMemberStatuses.FirstOrDefaultAsync(s => s.Id == status.Id);
        if (existing == null) return 0;

        existing.Name = status.Name;
        existing.DisplayName = status.DisplayName;
        existing.SortOrder = status.SortOrder;

        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }

    public async Task<int> DeleteAsync(int id)
    {
        var existing = await _context.GroupMemberStatuses.FirstOrDefaultAsync(s => s.Id == id);
        if (existing == null) return 0;

        _context.GroupMemberStatuses.Remove(existing);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }
}
