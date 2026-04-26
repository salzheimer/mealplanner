using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(int id);
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<IEnumerable<Recipe>> GetByOwnerIdAsync(int userId);
    Task<Recipe?> CreateAsync(Recipe recipe);
    Task<bool> UpdateAsync(Recipe recipe);
    Task<bool> DeleteAsync(int id);
}