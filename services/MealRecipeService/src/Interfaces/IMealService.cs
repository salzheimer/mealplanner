using Shared.Models;

namespace MealRecipeService.Interfaces;

public interface IMealService
{
    // Meal operations
    Task<Result<MealDto>> GetMealByIdAsync(int userId, int id);
    Task<Result<MealDto>> CreateMealAsync(int userId, MealCreateDto mealDto);
    Task<Result<MealDto>> UpdateMealAsync(int userId, MealUpdateDto mealDto);
    Task<Result<bool>> DeleteMealAsync(int userId, int id);
    Task<Result<MealDto>> CloneMealAsync(int userId, int mealId);
    // MealItem operations
    Task<Result<MealItemDto>> AddMealItemAsync(int userId, MealItemCreateDto mealItemDto);
    Task<Result<MealItemDto>> UpdateMealItemAsync(int userId, MealItemUpdateDto mealItemDto);
    Task<Result<IEnumerable<MealItemDto>>> GetMealItemByMealIdAsync(int userId, int mealId);
    Task<Result<bool>> DeleteMealItemAsync(int userId, int mealItemId);
    // Meal share operations
    Task<Result<ResourcePermissionDto>> ShareMealAsync(int userId, int mealId, ShareRequestDto request);
}
