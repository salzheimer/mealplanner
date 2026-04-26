using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;

public interface IMealItemRepository
{    Task<MealItem?> GetByIdAsync(int id);
    Task<IEnumerable<MealItem>> GetByMealIdAsync(int mealId);
    Task<MealItem?> CreateAsync(MealItem mealItem);
    Task<bool> UpdateAsync(MealItem mealItem);
    Task<bool> DeleteAsync(int id);
}