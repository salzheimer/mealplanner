using Microsoft.EntityFrameworkCore;
using IdentityService.Models;
using IdentityService.Services;

namespace IdentityService.Repositories;

public class GroupMemberStatusTypeRepository : Interfaces.IGroupMemberStatusTypeRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILookupCache _lookupCache;

    public GroupMemberStatusTypeRepository(IdentityDbContext context, ILookupCache lookupCache)
    {
        _context = context;
        _lookupCache = lookupCache;
    }

    public async Task<List<GroupMemberStatusType>> GetAllAsync()
    {
        return await _context.GroupMemberStatusTypes.ToListAsync();
    }

    public async Task<GroupMemberStatusType?> GetByIdAsync(int id)
    {
        return await _context.GroupMemberStatusTypes.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<int> CreateAsync(GroupMemberStatusType status)
    {
        _context.GroupMemberStatusTypes.Add(status);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }

    public async Task<int> UpdateAsync(GroupMemberStatusType status)
    {
        var existing = await _context.GroupMemberStatusTypes.FirstOrDefaultAsync(s => s.Id == status.Id);
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
        var existing = await _context.GroupMemberStatusTypes.FirstOrDefaultAsync(s => s.Id == id);
        if (existing == null) return 0;

        _context.GroupMemberStatusTypes.Remove(existing);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }
}
