using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IGroupMemberRepository
{
    Task<GroupMember?> CreateGroupMemberAsync(GroupMember member);
    Task<bool> UpdateGroupMemberAsync(GroupMember member);
    Task<bool> DeleteGroupMemberAsync(Guid groupMemberId);

    Task<GroupMember?> GetGroupMemberByIdAsync(Guid groupMemberId);
    Task<IEnumerable<GroupMember>> GetGroupMembersByGroupAsync(Guid groupId);
    Task<IEnumerable<GroupMember>> GetAllGroupMembersAsync();
    Task<IEnumerable<GroupMember>> GetGroupMembersByUserIdAsync(Guid userId);
    Task<GroupMember?> GetGroupMemberAsync(Guid userId, Guid groupId);

}