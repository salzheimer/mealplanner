using IdentityService.Contracts;
using Shared.Models;

namespace IdentityService.Interfaces;

public interface IGroupService
{
    Task<Result<GroupResponse>>GetGroup(Guid groupId);
    Task<Result<GroupResponse>>AddGroup(CreateGroupRequest request);
    Task<Result<GroupResponse>>UpdateGroup(UpdateGroupRequest request);
    Task<Result<bool>>DeleteGroup(Guid groupId);
    Task<Result<IEnumerable<GroupResponse>>>GetGroupsUserBelongs(Guid userId);

    //Group Member
    Task<Result<IEnumerable<GroupMemberSummaryResponse>>>GetGroupMembers(Guid groupId);
    Task<Result<IEnumerable<GroupMemberResponse>>>GetAllGroupMembers();
    Task<Result<GroupMemberResponse>>GetGroupMember(Guid userId, Guid groupId);

    Task<Result<GroupMemberResponse>>AddGroupMember(CreateGroupMemberRequest request);
    Task<Result<GroupMemberResponse>>UpdateGroupMember(UpdateGroupMemberRequest request);
    Task<Result<bool>>DeleteGroupMember(Guid groupMemberId);
    

}