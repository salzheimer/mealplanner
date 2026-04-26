using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;

public interface IMealShareRepository
{
    Task<MealShare?> GetByIdAsync(int id);
    Task<IEnumerable<MealShare>> GetByMealIdAsync(int mealId);
    Task<IEnumerable<MealShare>> GetBySharedWithUserIdAsync(int userId);
    Task<IEnumerable<MealShare>> GetBySharedByUserIdAsync(int userId);
    Task<IEnumerable<MealShare>> GetBySharedWithGroupIdAsync(int groupId);
    Task<MealShare?> CreateAsync(MealShare mealShare);
    Task<bool> UpdateAsync(MealShare mealShare);
    Task<bool> DeleteAsync(int id);
}