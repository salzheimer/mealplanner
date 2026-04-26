using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeShareRepository
{
    Task<RecipeShare?> GetByIdAsync(int id);
    Task<IEnumerable<RecipeShare>> GetByRecipeIdAsync(int recipeId);
    Task<IEnumerable<RecipeShare>> GetBySharedWithUserIdAsync(int userId);
    Task<IEnumerable<RecipeShare>> GetBySharedWithGroupIdAsync(int groupId);
    Task<RecipeShare?> CreateAsync(RecipeShare share);
    Task<bool> UpdateAsync(RecipeShare share);
    Task<bool> DeleteAsync(int id);
}