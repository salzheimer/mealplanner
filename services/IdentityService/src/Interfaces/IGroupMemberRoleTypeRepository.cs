using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IGroupMemberRoleTypeRepository
{
    Task<List<GroupMemberRoleType>> GetAllAsync();
    Task<GroupMemberRoleType?> GetByIdAsync(int id);
    Task<GroupMemberRoleType?> GetByName(string name);
    Task<int> CreateAsync(GroupMemberRoleType role);
    Task<int> UpdateAsync(GroupMemberRoleType role);
    Task<int> DeleteAsync(int id);
}
