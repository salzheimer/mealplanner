using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeComponentRepository
{
    Task<RecipeComponent?> GetByIdAsync(Guid id);
    Task<IEnumerable<RecipeComponent>> GetChildrenRecipesAsync(Guid parentRecipeId);
    Task<RecipeComponent?> CreateAsync(RecipeComponent component);
    Task<bool> UpdateAsync(RecipeComponent component);
    Task<bool> DeleteAsync(Guid id);
}