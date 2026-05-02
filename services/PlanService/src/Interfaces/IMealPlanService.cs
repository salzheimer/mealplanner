using Shared.Models;

namespace PlanService.Interfaces;

public interface IMealPlanService
{
    // MealPlan management
    Task<Result<MealPlanDto?>> CreateMealPlanAsync(MealPlanCreateDto mealPlan);
    Task<Result<MealPlanDto?>> GetMealPlanByIdAsync(int id);
    Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansForUserAsync(int userId);
    Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByStartDateAsync(DateTime startDate);
    Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByEndDateAsync(DateTime endDate);
    Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Result<MealPlanDto>> UpdateMealPlanAsync(MealPlanUpdateDto mealPlan);
    Task<Result<bool>> DeleteMealPlanAsync(int id);

    // MealItemPlan management
    Task<Result<MealItemPlanDto?>> AddMealItemToPlanAsync(MealItemPlanCreateDto mealItemPlan);
    Task<Result<IEnumerable<MealItemPlanDto>>> GetMealItemsForMealPlanAsync(int planId);

    Task<Result<MealItemPlanDto>> UpdateMealItemInPlanAsync(MealItemPlanUpdateDto mealItemPlan);
    Task<Result<bool>> RemoveMealItemFromPlanAsync(int mealItemPlanId);

}