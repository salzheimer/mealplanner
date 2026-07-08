using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IGroupRepository
{
    Task<Group?> CreateGroupAsync(Group group);
    Task<bool> UpdateGroupAsync(Group group);
    Task<bool> DeleteGroupAsync(Guid groupId);

    Task<IEnumerable<Group>> GetAllGroupsAsync();
    Task<Group?> GetGroupByIdAsync(Guid groupId);

    Task<IEnumerable<Group>> GetUserGroupsAsync(Guid userId);

}