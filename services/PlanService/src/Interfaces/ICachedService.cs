
using PlanService.Models;
using Shared.Models;

namespace PlanService.Interfaces;

public interface ICachedService
{
    //Cached User 
    Task<Result<CachedUser>> AddCachedUser(UserChanged user);
    Task<Result<CachedUser>> UpdateCachedUser(UserChanged user);
    Task<Result<bool>> DeleteCachedUser(Guid userId);

    //Cached Group
    Task<Result<CachedGroup>> AddCachedGroup(GroupChanged group);
    Task<Result<CachedGroup>> UpdateCachedGroup(GroupChanged group);
    Task<Result<bool>> DeleteCachedGroup(Guid groupId);

    //Cached Group Member
    Task<Result<CachedGroupMember>> AddCachedGroupMember(GroupMembershipChanged groupMember);
    Task<Result<CachedGroupMember>> UpdateCachedGroupMember(GroupMembershipChanged groupMember);
    Task<Result<CachedGroupMember>> UpdateCachedGroupMemberStatus(GroupMembershipChanged groupMember);
    Task<Result<bool>> DeleteCachedGroupMember(Guid gropMemberId);

    Task<bool> IsIdentityCacheEmptyAsync();
}