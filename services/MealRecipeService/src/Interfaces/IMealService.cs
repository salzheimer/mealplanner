using Shared.Models;
using MealRecipeService.Contracts;

namespace MealRecipeService.Interfaces;

public interface IMealService
{
    // Meal operations
    Task<Result<MealDetailResponse>> GetMealByIdAsync(Guid userId, Guid mealId);
    Task<Result<IEnumerable<MealSummaryResponse>>> GetAllMealsAsync(Guid userId);
    Task<Result<IEnumerable<MealSummaryResponse>>> GetMealsSharedWithMeAsync(Guid userId);
    Task<Result<MealDetailResponse>> CreateMealAsync(Guid userId, CreateMealRequest MealDetailResponse);
    Task<Result<MealDetailResponse>> UpdateMealAsync(Guid userId, UpdateMealRequest MealDetailResponse);
    Task<Result<bool>> DeleteMealAsync(Guid userId, Guid mealId);
    Task<Result<MealDetailResponse>> CloneMealAsync(Guid userId, Guid mealId);
    // MealItem operations
    Task<Result<MealItemDetailResponse>> AddMealItemAsync(Guid userId, CreateMealItemRequest MealItemDetailResponse);
    Task<Result<MealItemDetailResponse>> UpdateMealItemAsync(Guid userId, UpdateMealItemRequest MealItemDetailResponse);
    Task<Result<IEnumerable<MealItemDetailResponse>>> GetMealItemsByMealIdAsync(Guid userId, Guid mealId);
    Task<Result<bool>> DeleteMealItemAsync(Guid userId, Guid mealItemId);
    // Meal share operations
    Task<Result<ShareMealResponse>> ShareMealAsync(ShareMealRequest shareMealRequest);
}
