using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IGroupMemberStatusRepository
{
    Task<List<GroupMemberStatus>> GetAllAsync();
    Task<GroupMemberStatus?> GetByIdAsync(int id);
    Task<int> CreateAsync(GroupMemberStatus status);
    Task<int> UpdateAsync(GroupMemberStatus status);
    Task<int> DeleteAsync(int id);
}
