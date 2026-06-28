using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;

public interface IMealItemRepository
{    
    Task<MealItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<MealItem>> GetByMealIdAsync(Guid mealId);
    Task<MealItem?> CreateAsync(MealItem mealItem);
    Task<bool> UpdateAsync(MealItem mealItem);
    Task<bool> DeleteAsync(Guid id);
}

public interface IMealItemTypeRepository
{
    Task<MealItemType?> GetByIdAsync(int id);
    Task<MealItemType?> GetByNameAsync(string name);
    Task<IEnumerable<MealItemType>> GetAllAsync();
    Task<MealItemType?> CreateAsync(MealItemType mealItemType);
    Task<bool> UpdateAsync(MealItemType mealItemType);
    Task<bool> DeleteAsync(int id);
}