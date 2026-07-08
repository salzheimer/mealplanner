using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IGroupMemberStatusTypeRepository
{
    Task<List<GroupMemberStatusType>> GetAllAsync();
    Task<GroupMemberStatusType?> GetByIdAsync(int id);
    Task<int> CreateAsync(GroupMemberStatusType status);
    Task<int> UpdateAsync(GroupMemberStatusType status);
    Task<int> DeleteAsync(int id);
}
