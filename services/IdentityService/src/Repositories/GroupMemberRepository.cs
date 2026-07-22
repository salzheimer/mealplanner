using IdentityService.Interfaces;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Repositories;

public class GroupMemberRepository(IdentityDbContext context) : IGroupMemberRepository
{
    public async Task<GroupMember?> CreateGroupMemberAsync(GroupMember member)
    {
        context.GroupMembers.Add(member);
        var result = await context.SaveChangesAsync();
        if (result <= 0) return null;
        return member;
    }

    public async Task<bool> UpdateGroupMemberAsync(GroupMember member)
    {
        context.Entry(member).State = EntityState.Modified;

        var result = await context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteGroupMemberAsync(Guid groupMemberId)
    {
        var existing = await context.GroupMembers.FindAsync(groupMemberId);
        if (existing == null) return false;
        context.GroupMembers.Remove(existing);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<GroupMember?> GetGroupMemberByIdAsync(Guid groupMemberId)
    {
        return await context.GroupMembers
            .Include(m => m.GroupMemberRoleType)
            .Include(m => m.GroupMemberStatusType)
            .FirstOrDefaultAsync(m => m.GroupMemberId == groupMemberId);
    }

    public async Task<IEnumerable<GroupMember>> GetGroupMembersByGroupAsync(Guid groupId)
    {
        return await context.GroupMembers
            .Include(m => m.GroupMemberRoleType)
            .Include(m => m.GroupMemberStatusType)
            .Where(m => m.GroupId == groupId)
            .ToListAsync();
    }

    // Used in syncing functions across the services
    public async Task<IEnumerable<GroupMember>> GetAllGroupMembersAsync()
    {
        return await context.GroupMembers
            .Include(m => m.GroupMemberRoleType)
            .Include(m => m.GroupMemberStatusType)
            .ToListAsync();
    }

    public async Task<IEnumerable<GroupMember>> GetGroupMembersByUserIdAsync(Guid userId)
    {
        return await context.GroupMembers
            .Include(m => m.GroupMemberRoleType)
            .Include(m => m.GroupMemberStatusType)
            .Where(m => m.UserId == userId)
            .ToListAsync();
    }

    public async Task<GroupMember?> GetGroupMemberAsync(Guid userId, Guid groupId)
    {
        return await context.GroupMembers
            .Include(m => m.GroupMemberRoleType)
            .Include(m => m.GroupMemberStatusType)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);
    }
}
