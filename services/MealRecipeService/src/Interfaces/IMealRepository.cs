using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IMealRepository
{
    Task<Meal?> GetByIdAsync(Guid id);
    Task<IEnumerable<Meal>> GetByOwnerIdAsync(Guid userId);
    Task<IEnumerable<Meal>> GetByIdsAsync(HashSet<Guid> sharedMealIds);
    Task<Meal?> CreateAsync(Meal meal);
    Task<bool> UpdateAsync(Meal meal);
    Task<bool> DeleteAsync(Guid id);
}
public interface IMealTypeRepository
{
    Task<MealType?> GetByIdAsync(int id);
    Task<MealType?> GetByNameAsync(string name);
    Task<IEnumerable<MealType>> GetAllAsync();
    Task<MealType?> CreateAsync(MealType mealType);
    Task<bool> UpdateAsync(MealType mealType);
    Task<bool> DeleteAsync(int id);
}