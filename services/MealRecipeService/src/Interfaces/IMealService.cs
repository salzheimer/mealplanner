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
    // Meal share operations
    Task<Result<ResourcePermissionDto>> ShareMealAsync(int userId, int mealId, ShareRequestDto request);
}