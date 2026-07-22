using System.Reflection.Metadata.Ecma335;
using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories;

public class GroupRepository(IdentityDbContext context) : IGroupRepository
{

    public async Task<Group?> CreateGroupAsync(Group group)
    {
        context.Groups.Add(group);
        var result = await context.SaveChangesAsync();
        if (result <= 0) return null!;
        return group;
    }

    public async Task<bool> UpdateGroupAsync(Group group)
    {
        context.Entry(group).State = EntityState.Modified;
        var result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteGroupAsync(Guid groupId)
    {
        var existing = await context.Groups.FindAsync(groupId);
        if (existing == null) return false;
        context.Groups.Remove(existing);
        return await context.SaveChangesAsync() > 0;
    }
    //Used in syncing functions across services
    public async Task<IEnumerable<Group>> GetAllGroupsAsync()
    {
        return await context.Groups.ToListAsync();
    }

    public async Task<Group?> GetGroupByIdAsync(Guid groupId)
    {
        return await context.Groups.FindAsync(groupId);
    }
    /// <summary>
    /// Get groups a user is listed as 'active'
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>a collection of groups the user is active</returns>
    public async Task<IEnumerable<Group>> GetUserGroupsAsync(Guid userId)
    {
        return await context.Groups
            .Where(g => g.GroupMembers.Any(m => m.UserId == userId && m.GroupMemberStatusType.Name == "active"))
            .ToListAsync();
    }
}
