using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IMealRepository
{
    Task<Meal?> GetByIdAsync(int id);
    
    Task<Meal?> CreateAsync(Meal meal);
    Task<bool> UpdateAsync(Meal meal);
    Task<bool> DeleteAsync(int id);
}