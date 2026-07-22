
using System.Text.RegularExpressions;
using MealRecipeService.Interfaces;
using MealRecipeService.Models;
using Npgsql.Internal;
using Shared.Models;

namespace MealRecipeService.Services;

public class CachedService : Interfaces.ICachedService
{
    private readonly ICachedUserRepository _userRepository;
    private readonly ICachedGroupRepository _groupRepository;
    private readonly ICachedGroupMemberRepository _groupMemberRepository;


    public CachedService(ICachedUserRepository userRepository, ICachedGroupRepository groupRepository, ICachedGroupMemberRepository groupMemberRepository)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
    }

    //Cached Users


    public async Task<Result<CachedUser>> AddCachedUser(UserChanged user)
    {

        var existingUser = await _userRepository.GetByIdAsync(user.UserId);
        if (existingUser == null)
        {
            var entity = new CachedUser()
            {
                Id = user.UserId,
                DisplayName = user.DisplayName,
                SyncedAt = DateTimeOffset.UtcNow,
                SourceUpdatedAt =user.SourceUpdatedAt
            };

            var result = await _userRepository.CreateAsync(entity);
            if (result == null)
                return Result<CachedUser>.Failure(CachedUserErrors.UnableToCreate);

            return Result<CachedUser>.Success(result);
        }
        else
        {
            return await UpdateCachedUser(user);
        }
    }
    public async Task<Result<CachedUser>> UpdateCachedUser(UserChanged user)
    {
        var entity = new CachedUser()
        {
            Id = user.UserId,
            DisplayName = user.DisplayName,
            SyncedAt = DateTimeOffset.UtcNow,
            SourceUpdatedAt = user.SourceUpdatedAt
        };

        var updated = await _userRepository.UpdateAsync(entity);
        if (updated == false)
            return Result<CachedUser>.Failure(CachedUserErrors.UnableToUpdate);

        return Result<CachedUser>.Success(entity);
    }
    public async Task<Result<bool>> DeleteCachedUser(Guid userId)
    {
        if (await _userRepository.DeleteAsync(userId))
        {
            return Result<bool>.Success(true);
        }
        else
        {
            return Result<bool>.Failure(CachedUserErrors.UnableToDelete);
        }
    }

    //Cached Group
    public async Task<Result<CachedGroup>> AddCachedGroup(GroupChanged group)
    {
        var existing = await _groupRepository.GetByIdAsync(group.GroupId);
        if (existing == null)
        {
            var entity = new CachedGroup()
            {
                Id = group.GroupId,
                
                GroupName = group.GroupName,
                SyncedAt = DateTimeOffset.UtcNow,
                SourceUpdatedAt = group.SourceUpdatedAt
            };

            var result = await _groupRepository.CreateAsync(entity);

            if (result == null) return Result<CachedGroup>.Failure(CachedGroupErrors.UnableToCreate);

            return Result<CachedGroup>.Success(result);
        }
        else
        {
            return await UpdateCachedGroup(group);
        }
    }

    public async Task<Result<CachedGroup>> UpdateCachedGroup(GroupChanged group)
    {
        var entity = new CachedGroup()
        {
            Id = group.GroupId,
            GroupName = group.GroupName,
            SyncedAt = DateTimeOffset.UtcNow
        };

        var updated = await _groupRepository.UpdateAsync(entity);

        if (updated == false)
        {
            return Result<CachedGroup>.Failure(CachedGroupErrors.UnableToUpdate);
        }
        return Result<CachedGroup>.Success(entity);
    }
    public async Task<Result<bool>> DeleteCachedGroup(Guid groupId)
    {
        if (await _groupRepository.DeleteAsync(groupId))
        {
            return Result<bool>.Success(true);
        }
        else
        {
            return Result<bool>.Failure(CachedGroupErrors.UnableToDelete);
        }
    }
    //Cached Group Member
    public async Task<Result<CachedGroupMember>> AddCachedGroupMember(GroupMembershipChanged groupMember)
    {
        var existing = await _groupMemberRepository.GetByIdAsync(groupMember.GroupMemberId);
        if (existing == null)
        {
            var entity = new CachedGroupMember()
            {
                Id = groupMember.GroupMemberId,
                GroupId = groupMember.GroupId,
                UserId = groupMember.UserId,
                RoleName = groupMember.RoleName,
                StatusName = groupMember.StatusName,
                SyncedAt = DateTimeOffset.UtcNow,
                SourceUpdatedAt = groupMember.SourceUpdatedAt
            };

            var result = await _groupMemberRepository.CreateAsync(entity);
            if (result == null) return Result<CachedGroupMember>.Failure(CachedGroupMembershipErrors.UnableToCreate);

            return Result<CachedGroupMember>.Success(result);
        }
        else
        {
            return await UpdateCachedGroupMember(groupMember);
        }
    }



    public async Task<Result<bool>> DeleteCachedGroupMember(Guid groupMemberId)
    {
        var deleted = await _groupMemberRepository.DeleteAsync(groupMemberId);
        if (deleted)
        {
            return Result<bool>.Success(true);
        }
        else
        {
            return Result<bool>.Failure(CachedGroupMembershipErrors.UnableToDelete);
        }
    }





    public async Task<Result<CachedGroupMember>> UpdateCachedGroupMember(GroupMembershipChanged groupMember)
    {
        var entity = new CachedGroupMember()
        {
            Id = groupMember.GroupMemberId,
            GroupId = groupMember.GroupId,
            UserId = groupMember.UserId,
            RoleName = groupMember.RoleName,
            StatusName = groupMember.StatusName,
            SyncedAt = DateTimeOffset.UtcNow,
            SourceUpdatedAt = groupMember.SourceUpdatedAt
        };

        var result = await _groupMemberRepository.UpdateAsync(entity);
        if (result)
        {
            return Result<CachedGroupMember>.Success(entity);
        }
        else
        {
            return Result<CachedGroupMember>.Failure(CachedGroupMembershipErrors.UnableToUpdate);
        }

    }

    public Task<Result<CachedGroupMember>> UpdateCachedGroupMemberStatus(GroupMembershipChanged groupMember)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsIdentityCacheEmptyAsync()
    {
        // If ANY table is empty, we consider the cache incomplete
        bool hasUsers = await _userRepository.AnyAsync();
        bool hasGroups = await _groupRepository.AnyAsync();
        bool hasMembers = await _groupMemberRepository.AnyAsync();

        return !hasUsers || !hasGroups || !hasMembers;
    }
}