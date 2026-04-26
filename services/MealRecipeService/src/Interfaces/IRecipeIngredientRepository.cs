using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeIngredientRepository
{
    Task<RecipeIngredient?> GetByIdAsync(int id);
    Task<IEnumerable<RecipeIngredient>> GetByRecipeIdAsync(int recipeId);
    Task<RecipeIngredient?> CreateAsync(RecipeIngredient ingredient);
    Task<bool> UpdateAsync(RecipeIngredient ingredient);
    Task<bool> DeleteAsync(int id);
}