using PlanService.Models;

namespace PlanService.Interfaces;
public interface ICachedUserRepository
{
    Task<CachedUser?> GetByIdAsync(Guid id);     
    Task<CachedUser?> CreateAsync(CachedUser user);
    Task<bool> UpdateAsync(CachedUser user);
    Task<bool> DeleteAsync(Guid id);
}
public interface ICachedGroupRepository
{
    Task<CachedGroup?> GetByIdAsync(Guid id);
    Task<CachedGroup?> CreateAsync(CachedGroup group);
    Task<bool> UpdateAsync(CachedGroup group);
    Task<bool> DeleteAsync(Guid id);
}