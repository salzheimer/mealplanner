using System.IO.Pipelines;
using Shared.Models;

namespace MealRecipeService.Interfaces;

public interface IMealService
{
    // Meal operations
    Task<Result<MealDto>> GetMealByIdAsync(int id);
    Task<Result<MealDto>> CreateMealAsync(int userId, MealCreateDto mealDto);
    Task<Result<MealDto>> UpdateMealAsync(int userId, MealUpdateDto mealDto);
    Task<Result<bool>> DeleteMealAsync(int userId, int id);
   // MealItem operations
    Task<Result<MealItemDto>> AddMealItemAsync(MealItemCreateDto mealItemDto);
    Task<Result<MealItemDto>> UpdateMealItemAsync(MealItemUpdateDto mealItemDto);
    Task<Result<IEnumerable<MealItemDto>>> GetMealItemByMealIdAsync(int mealId);
    Task<Result<bool>> DeleteMealItemAsync(int mealItemId);

    // MealShare operations
    Task<Result<MealShareDto>> ShareMealAsync(MealShareCreateDto mealShareDto);
    Task<Result<MealShareDto>> UpdateMealShareAsync(MealShareUpdateDto mealShareDto);
    Task<Result<bool>> DeleteMealShareAsync(int mealShareId);   
}