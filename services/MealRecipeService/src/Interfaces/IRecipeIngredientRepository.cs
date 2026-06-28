using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeIngredientRepository
{
    Task<RecipeIngredient?> GetByIdAsync(Guid id);
    Task<IEnumerable<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId);
    Task<RecipeIngredient?> CreateAsync(RecipeIngredient ingredient);
    Task<bool> UpdateAsync(RecipeIngredient ingredient);
    Task<bool> DeleteAsync(Guid id);
}