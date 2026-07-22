using IdentityService.Contracts;
using Shared.Models;

namespace IdentityService.Interfaces;

public interface IGroupService
{
    Task<Result<IEnumerable<GroupResponse>>> GetAllGroupsAsync();
    Task<Result<GroupResponse>>GetGroup(Guid currentUserId,Guid groupId);
    Task<Result<GroupResponse>>AddGroup(Guid currentUserId,CreateGroupRequest request);
    Task<Result<GroupResponse>>UpdateGroup(Guid currentUserId,UpdateGroupRequest request);
    Task<Result<bool>>DeleteGroup(Guid currentUserId,Guid groupId);
    Task<Result<IEnumerable<GroupResponse>>>GetGroupsUserBelongs(Guid currentUserId,Guid userId);

    //Group Member
    Task<Result<IEnumerable<GroupMemberSummaryResponse>>>GetGroupMembers(Guid currentUserId,Guid groupId);
    Task<Result<IEnumerable<GroupMemberResponse>>>GetAllGroupMembersAsync();
    Task<Result<GroupMemberResponse>>GetGroupMember(Guid currentUserId,Guid userId, Guid groupId);

    Task<Result<GroupMemberResponse>>AddGroupMember(Guid currentUserId,CreateGroupMemberRequest request);
    Task<Result<GroupMemberResponse>>UpdateGroupMember(Guid currentUserId,UpdateGroupMemberRequest request);
    Task<Result<bool>>DeleteGroupMember(Guid currentUserId,Guid groupMemberId);
    

}