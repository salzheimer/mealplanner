using IdentityService.Contracts;
using IdentityService.Interfaces;
using IdentityService.Models;
using Rebus.Bus;
using Shared.Models;

namespace IdentityService.Services;

public class GroupService(IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, IGroupMemberRoleTypeRepository groupMemberRoleTypeRepository, IGroupMemberStatusTypeRepository groupMemberStatusTypeRepository, IBus bus) : IGroupService
{
    //Group
    public async Task<Result<GroupResponse>> AddGroup(Guid currentUserId, CreateGroupRequest request)
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        var entity = new Models.Group()
        {
            Name = request.Name,
            CreatedBy = currentUserId,
            CreatedAt =createdAt,
            UpdatedAt = createdAt
        };
        var group = await groupRepository.CreateGroupAsync(entity);
        if (group == null)
            return Result<GroupResponse>.Failure(GroupErrors.UnableToCreate);

        //send message 
        await bus.Publish(new GroupChanged
        {
            GroupId = group.GroupId,
            GroupName = group.Name,
            SourceUpdatedAt = createdAt,
            Action = "Created"
        });

        //assign group creator group owner role
        var role = await groupMemberRoleTypeRepository.GetByName("owner");
        var status = await groupMemberStatusTypeRepository.GetByName("active");
        if (role == null || status == null)
            return Result<GroupResponse>.Failure(GroupMemberErrors.UnableToCreate);
        var groupOwner = new GroupMember()
        {
            //there is not invite for group creator
            GroupId = group.GroupId,
            UserId = currentUserId,
            RoleId = role.Id,
            StatusId = status.Id,
            JoinedAt = createdAt,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
        var member = await groupMemberRepository.CreateGroupMemberAsync(groupOwner);
        if (member == null)
            return Result<GroupResponse>.Failure(GroupMemberErrors.UnableToCreate);

        await bus.Publish(new GroupMembershipChanged { GroupMemberId = member.GroupMemberId, GroupId = member.GroupId, UserId = member.UserId, RoleName = role.Name, StatusName = status.Name, SourceUpdatedAt=member.UpdatedAt, Action = "Created" });

        return Result<GroupResponse>.Success(new GroupResponse(group.GroupId, group.Name, group.CreatedBy, group.CreatedAt));
    }

    public async Task<Result<bool>> DeleteGroup(Guid currentUserId, Guid groupId)
    {
        var deleted = await groupRepository.DeleteGroupAsync(groupId);
        if (!deleted) return Result<bool>.Failure(GroupErrors.UnableToDelete);
        //send message
        await bus.Publish(new GroupChanged { GroupId = groupId, SourceUpdatedAt = DateTimeOffset.UtcNow, Action = "Deleted" });
        return Result<bool>.Success(true);
    }
    public async Task<Result<GroupResponse>> GetGroup(Guid currentUserId, Guid groupId)
    {
        var group = await groupRepository.GetGroupByIdAsync(groupId);
        if (group == null)
        {
            return Result<GroupResponse>.Failure(GroupErrors.NotFound);
        }
        return Result<GroupResponse>.Success(ToGroupResponse(group));
    }
    public async Task<Result<IEnumerable<GroupResponse>>> GetAllGroupsAsync()
    {
        IEnumerable<GroupResponse> groupResponses = Enumerable.Empty<GroupResponse>();

        var groups = await groupRepository.GetAllGroupsAsync();
        if (groups.Any())
        {
            groupResponses = groups.Select(g => new GroupResponse(GroupId: g.GroupId, Name: g.Name, CreatedBy: g.CreatedBy, CreatedAt: g.CreatedAt));
        }

        return Result<IEnumerable<GroupResponse>>.Success(groupResponses);
    }


    public async Task<Result<GroupResponse>> UpdateGroup(Guid currentUserId, UpdateGroupRequest request)
    {
        var entity = new Group()
        {
            GroupId = request.GroupId,
            Name = request.Name,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        if (await groupRepository.UpdateGroupAsync(entity))
        {
            //send message
            await bus.Publish(new GroupChanged
            {
                GroupId = request.GroupId,
                GroupName = request.Name,
                SourceUpdatedAt = DateTimeOffset.UtcNow,
                Action = "Deleted"
            });
            return Result<GroupResponse>.Success(ToGroupResponse(entity));
        }
        return Result<GroupResponse>.Failure(GroupErrors.UnableToUpdate);

    }


    //Group Members
    public async Task<Result<GroupMemberResponse>> AddGroupMember(Guid currentUserId, CreateGroupMemberRequest request)
    {
        DateTimeOffset createdAt = DateTimeOffset.UtcNow;

        var entity = new GroupMember()
        {
            GroupId = request.GroupId,
            RoleId = request.RoleId,
            StatusId = request.StatusId,
            InvitedAt = createdAt,
            InvitedByUserId = currentUserId,
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };

        var groupMember = await groupMemberRepository.CreateGroupMemberAsync(entity);
        if (groupMember == null)
        {
            return Result<GroupMemberResponse>.Failure(GroupMemberErrors.UnableToCreate);
        }
        //send message
        await bus.Publish(new GroupMembershipChanged
        {
            GroupId = groupMember.GroupId,
            GroupMemberId = groupMember.GroupMemberId,
            UserId = groupMember.UserId,
            RoleName = groupMember.GroupMemberRoleType.Name,
            StatusName = groupMember.GroupMemberStatusType.Name,
            SourceUpdatedAt = groupMember.UpdatedAt,
            Action = "Created"
        });
        return Result<GroupMemberResponse>.Success(ToGroupMemberResponse(groupMember));
    }
    public async Task<Result<bool>> DeleteGroupMember(Guid currentUserId, Guid groupMemberId)
    {
        if (await groupMemberRepository.DeleteGroupMemberAsync(groupMemberId))
        {
            await bus.Publish(new GroupMembershipChanged { GroupMemberId = groupMemberId, Action = "Deleted" });
            return Result<bool>.Success(true);
        }
        return Result<bool>.Failure(GroupMemberErrors.UnableToDelete);
    }

    public async Task<Result<IEnumerable<GroupMemberResponse>>> GetAllGroupMembersAsync()
    {
        IEnumerable<GroupMemberResponse> groupMemberResponses = Enumerable.Empty<GroupMemberResponse>();
        var groupMembers = await groupMemberRepository.GetAllGroupMembersAsync();
        if (groupMembers.Any())
        {
            groupMemberResponses = groupMembers.Select(gm => new GroupMemberResponse(
                GroupMemberId: gm.GroupMemberId, GroupId: gm.GroupId, GroupName: gm.Group.Name, UserId: gm.UserId
            , GroupMemberRoleTypeId: gm.GroupMemberRoleType.Id, GroupMemberRoleTypeName: gm.GroupMemberRoleType.Name
            , GroupMemberStatusTypeId: gm.GroupMemberStatusType.Id, GroupMemberStatusTypeName: gm.GroupMemberStatusType.Name
            , InvitedAt: gm.InvitedAt, JoinedAt: gm.JoinedAt, RemovedAt: gm.RemovedAt));
        }

        return Result<IEnumerable<GroupMemberResponse>>.Success(groupMemberResponses);
    }
    public async Task<Result<GroupMemberResponse>> GetGroupMember(Guid currentUserId, Guid userId, Guid groupId)
    {
        var groupMember = await groupMemberRepository.GetGroupMemberAsync(userId, groupId);

        if (groupMember == null)
            return Result<GroupMemberResponse>.Failure(GroupMemberErrors.UnableToLocate);

        return Result<GroupMemberResponse>.Success(ToGroupMemberResponse(groupMember));
    }


    public async Task<Result<GroupMemberResponse>> UpdateGroupMember(Guid currentUserId, UpdateGroupMemberRequest request)
    {

        var entity = new GroupMember()
        {
            GroupMemberId = request.GroupMemberId,
            GroupId = request.GroupId,
            UserId = request.UserId,
            RoleId = request.GroupMemberRoleTypeId,
            StatusId = request.GroupMemberStatusTypeId,
            JoinedAt = request.JoinedAt,
            RemovedAt = request.RemovedAt,
            UpdatedAt = DateTimeOffset.UtcNow

        };
        if (await groupMemberRepository.UpdateGroupMemberAsync(entity))
        {
            var groupMember = await groupMemberRepository.GetGroupMemberByIdAsync(request.GroupMemberId);
            //send update message
            await bus.Publish(new GroupMembershipChanged
            {
                GroupId = groupMember.GroupId,
                GroupMemberId = groupMember.GroupMemberId,
                UserId = groupMember.UserId,
                RoleName = groupMember.GroupMemberRoleType.Name,
                StatusName = groupMember.GroupMemberStatusType.Name,
                SourceUpdatedAt = DateTimeOffset.UtcNow,
                Action = "Updated"
            });
            return Result<GroupMemberResponse>.Success(ToGroupMemberResponse(groupMember));
        }
        return Result<GroupMemberResponse>.Failure(GroupMemberErrors.UnableToUpdate);
    }

    public async Task<Result<IEnumerable<GroupMemberSummaryResponse>>> GetGroupMembers(Guid currentUserId, Guid groupId)
    {
        IEnumerable<GroupMemberSummaryResponse> groupMemberSummaryResponses = Enumerable.Empty<GroupMemberSummaryResponse>();
        var members = await groupMemberRepository.GetGroupMembersByGroupAsync(groupId);
        if (members.Any())
        {
            members.Select(gm => new GroupMemberSummaryResponse(
                GroupMemberId: gm.GroupMemberId,
                GroupId: gm.GroupId,
                GroupName: gm.Group.Name,
                UserId: gm.UserId,
                UserName: gm.User.DisplayName,
                GroupMemberRoleTypeId: gm.RoleId,
                GroupMemberRoleTypeName: gm.GroupMemberRoleType.Name,
                GroupMemberStatusTypeId: gm.StatusId,
                GroupMemberStatusTypeName: gm.GroupMemberStatusType.Name
            ));
        }
        return Result<IEnumerable<GroupMemberSummaryResponse>>.Success(groupMemberSummaryResponses);
    }

    public async Task<Result<IEnumerable<GroupResponse>>> GetGroupsUserBelongs(Guid currentUserId, Guid userId)
    {
        IEnumerable<GroupResponse> groupResponses = Enumerable.Empty<GroupResponse>();

        var gm = await groupMemberRepository.GetGroupMembersByUserIdAsync(userId);
        if (gm.Any())
        {

            groupResponses = gm.Select(gm => new GroupResponse(GroupId: gm.GroupId, Name: gm.Group.Name, CreatedBy: gm.Group.CreatedBy, CreatedAt: gm.Group.CreatedAt));
        }
        return Result<IEnumerable<GroupResponse>>.Success(groupResponses);
    }

    private GroupMemberResponse ToGroupMemberResponse(GroupMember gm)
    {
        return new GroupMemberResponse(
                 GroupMemberId: gm.GroupMemberId, GroupId: gm.GroupId, GroupName: gm.Group.Name, UserId: gm.UserId
             , GroupMemberRoleTypeId: gm.GroupMemberRoleType.Id, GroupMemberRoleTypeName: gm.GroupMemberRoleType.Name
             , GroupMemberStatusTypeId: gm.GroupMemberStatusType.Id, GroupMemberStatusTypeName: gm.GroupMemberStatusType.Name
             , InvitedAt: gm.InvitedAt, JoinedAt: gm.JoinedAt, RemovedAt: gm.RemovedAt);
    }
    private GroupResponse ToGroupResponse(Group group)
    {
        return new GroupResponse(GroupId: group.GroupId, Name: group.Name, CreatedBy: group.CreatedBy, CreatedAt: group.CreatedAt);
    }
}