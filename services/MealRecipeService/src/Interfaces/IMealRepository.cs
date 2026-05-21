using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IMealRepository
{
    Task<Meal?> GetByIdAsync(int id);
    Task<IEnumerable<Meal>> GetByOwnerIdAsync(int userId);
    Task<IEnumerable<Meal>> GetByIdsAsync(HashSet<int> sharedMealIds);
    Task<Meal?> CreateAsync(Meal meal);
    Task<bool> UpdateAsync(Meal meal);
    Task<bool> DeleteAsync(int id);
}