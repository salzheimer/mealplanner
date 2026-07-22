using Microsoft.EntityFrameworkCore;
using IdentityService.Models;
using IdentityService.Services;

namespace IdentityService.Repositories;

public class GroupMemberRoleTypeRepository : Interfaces.IGroupMemberRoleTypeRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILookupCache _lookupCache;

    public GroupMemberRoleTypeRepository(IdentityDbContext context, ILookupCache lookupCache)
    {
        _context = context;
        _lookupCache = lookupCache;
    }

    public async Task<List<GroupMemberRoleType>> GetAllAsync()
    {
        return await _context.GroupMemberRoleTypes.ToListAsync();
    }

    public async Task<GroupMemberRoleType?> GetByIdAsync(int id)
    {
        return await _context.GroupMemberRoleTypes.FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<GroupMemberRoleType?> GetByName(string name)
    {
       return await _context.GroupMemberRoleTypes.FirstOrDefaultAsync(r=>r.Name ==name);
    }
    public async Task<int> CreateAsync(GroupMemberRoleType role)
    {
        _context.GroupMemberRoleTypes.Add(role);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }

    public async Task<int> UpdateAsync(GroupMemberRoleType role)
    {
        var existing = await _context.GroupMemberRoleTypes.FirstOrDefaultAsync(r => r.Id == role.Id);
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
        var existing = await _context.GroupMemberRoleTypes.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return 0;

        _context.GroupMemberRoleTypes.Remove(existing);
        var result = await _context.SaveChangesAsync();
        await _lookupCache.RefreshAsync();
        return result;
    }
}
