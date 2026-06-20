using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IGroupMemberRoleRepository
{
    Task<List<GroupMemberRole>> GetAllAsync();
    Task<GroupMemberRole?> GetByIdAsync(int id);
    Task<int> CreateAsync(GroupMemberRole role);
    Task<int> UpdateAsync(GroupMemberRole role);
    Task<int> DeleteAsync(int id);
}
