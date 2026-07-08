using System.Text.RegularExpressions;
using IdentityService.Contracts;
using IdentityService.Interfaces;
using IdentityService.Models;
using Shared.Models;

namespace IdentityService.Services;

public class GroupService(IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository) : IGroupService
{
    public async Task<Result<GroupResponse>> AddGroup(CreateGroupRequest request)
    {
        var entity = new Models.Group()
        {
            Name = request.Name,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var group = await groupRepository.CreateGroupAsync(entity);
        if (group == null)
            return Result<GroupResponse>.Failure(GroupErrors.UnableToCreate);

        return Result<GroupResponse>.Success(new GroupResponse(group.GroupId, group.Name, group.CreatedBy, group.CreatedAt));
    }

    public Task<Result<GroupMemberResponse>> AddGroupMember(CreateGroupMemberRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteGroupMember(Guid groupMemberId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<GroupMemberResponse>>> GetAllGroupMembers()
    {
        throw new NotImplementedException();
    }

    public Task<Result<GroupResponse>> GetGroup(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<GroupMemberResponse>> GetGroupMember(Guid userId, Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<GroupMemberSummaryResponse>> GetGroupMembers(Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<GroupResponse>> GetGroupsUserBelongs(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<GroupResponse>> UpdateGroup(UpdateGroupRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<GroupMemberResponse>> UpdateGroupMember(UpdateGroupMemberRequest request)
    {
        throw new NotImplementedException();
    }

    Task<Result<IEnumerable<GroupMemberSummaryResponse>>> IGroupService.GetGroupMembers(Guid groupId)
    {
        throw new NotImplementedException();
    }

    Task<Result<IEnumerable<GroupResponse>>> IGroupService.GetGroupsUserBelongs(Guid userId)
    {
        throw new NotImplementedException();
    }
}