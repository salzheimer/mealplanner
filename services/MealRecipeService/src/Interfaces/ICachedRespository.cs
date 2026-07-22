using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface ICachedUserRepository
{
    Task<bool> AnyAsync();
    Task<CachedUser?> GetByIdAsync(Guid id);
     
    Task<CachedUser?> CreateAsync(CachedUser user);
    Task<bool> UpdateAsync(CachedUser user);
    Task<bool> DeleteAsync(Guid id);
}
public interface ICachedGroupRepository
{
    Task<bool> AnyAsync();
    Task<CachedGroup?> GetByIdAsync(Guid id);
    Task<CachedGroup?> CreateAsync(CachedGroup group);
    Task<bool> UpdateAsync(CachedGroup group);
    Task<bool> DeleteAsync(Guid id);
}

public interface ICachedGroupMemberRepository
{
    Task<bool> AnyAsync();
    Task<CachedGroupMember?> GetByIdAsync(Guid id);
    Task<CachedGroupMember?> CreateAsync(CachedGroupMember groupMember);
    Task<bool> UpdateAsync(CachedGroupMember groupMember);
    Task<bool> DeleteAsync(Guid id);
}