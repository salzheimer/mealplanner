using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id);
    Task<IEnumerable<Recipe>> GetAllAsync();
    Task<IEnumerable<Recipe>> GetByIdsAsync(HashSet<Guid> recipeIds);
    Task<IEnumerable<Recipe>> GetByOwnerIdAsync(Guid ownerId);
    Task<Recipe?> CreateAsync(Recipe recipe);
    Task<bool> UpdateAsync(Recipe recipe);
    Task<bool> DeleteAsync(Guid id);
    
}