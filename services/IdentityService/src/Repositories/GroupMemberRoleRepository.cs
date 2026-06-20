using Microsoft.EntityFrameworkCore;
using IdentityService.Models;
using IdentityService.Services;

namespace IdentityService.Repositories;

public class GroupMemberRoleRepository : Interfaces.IGroupMemberRoleRepository
{
    private readonly UserContext _context;
    private readonly ILookupCache _lookupCache;

    public GroupMemberRoleRepository(UserContext context, ILookupCache lookupCache)
    {
        _context = context;
        _lookupCache = lookupCache;
    }

    public async Task<List<GroupMemberRole>> GetAllAsync()
    {
        return await _context.GroupMemberRoles.ToListAsync();
    }

    public async Task<GroupMemberRole?> GetByIdAsync(int id)
    {
        return await _context.GroupMemberRoles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<int> CreateAsync(GroupMemberRole role)
    {
        _context.GroupMemberRoles.Add(role);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }

    public async Task<int> UpdateAsync(GroupMemberRole role)
    {
        var existing = await _context.GroupMemberRoles.FirstOrDefaultAsync(r => r.Id == role.Id);
        if (existing == null) return 0;

        existing.Name = role.Name;
        existing.DisplayName = role.DisplayName;
        existing.SortOrder = role.SortOrder;

        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }

    public async Task<int> DeleteAsync(int id)
    {
        var existing = await _context.GroupMemberRoles.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return 0;

        _context.GroupMemberRoles.Remove(existing);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }
}
