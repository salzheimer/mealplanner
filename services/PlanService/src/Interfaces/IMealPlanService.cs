using Shared.Models;

namespace PlanService.Interfaces;

public interface IMealPlanService
{
    // MealPlan management
    Task<Result<MealPlanDto?>> CreateMealPlanAsync(int userId, MealPlanCreateDto mealPlan);
    Task<Result<MealPlanDto?>> GetMealPlanByIdAsync(int userId, int id);
    Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansForUserAsync(int userId);
    Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByStartDateAsync(int userId, DateTime startDate);
    Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByEndDateAsync(int userId, DateTime endDate);
    Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
    Task<Result<MealPlanDto>> UpdateMealPlanAsync(int userId, MealPlanUpdateDto mealPlan);
    Task<Result<bool>> DeleteMealPlanAsync(int userId, int id);

    // MealItemPlan management
    Task<Result<MealItemPlanDto?>> AddMealItemToPlanAsync(int userId, int mealPlanId, MealItemPlanCreateDto mealItemPlan);
    Task<Result<IEnumerable<MealItemPlanDto>>> GetMealItemsForMealPlanAsync(int userId, int mealPlanId);

    Task<Result<MealItemPlanDto>> UpdateMealItemInPlanAsync(int userId, int mealPlanId, int mealItemId, MealItemPlanUpdateDto mealItemPlan);
    Task<Result<bool>> RemoveMealItemFromPlanAsync(int userId, int mealItemPlanId);

}