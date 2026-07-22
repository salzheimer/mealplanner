using PlanService.Contracts;
using Shared.Models;

namespace PlanService.Interfaces;

public interface IMealPlanService
{
    // MealPlan management
    Task<Result<PlanMealResponse?>> CreateMealPlanAsync(Guid userId, CreatePlanMealRequest mealPlan);
    Task<Result<PlanMealResponse?>> GetMealPlanByIdAsync(Guid userId, Guid planMealId);
    Task<Result<IEnumerable<PlanMealResponse>>> GetMealPlansForUserAsync(Guid userId);
    Task<Result<IEnumerable<PlanMealResponse>>> GetMealPlansByStartDateAsync(Guid userId, DateTime startDate);
    Task<Result<IEnumerable<PlanMealResponse>>> GetMealPlansByEndDateAsync(Guid userId, DateTime endDate);
    Task<Result<IEnumerable<PlanMealResponse>>> GetMealPlansByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<Result<PlanMealResponse>> UpdateMealPlanAsync(Guid userId, UpdatePlanMealRequest mealPlan);
    Task<Result<bool>> DeleteMealPlanAsync(Guid userId, Guid planMealId);

    // MealItemPlan management
    Task<Result<PlanMealItemResponse?>> AddMealItemToPlanAsync(Guid userId, Guid planMealId, CreatePlanMealItemRequest mealItemPlan);
    Task<Result<IEnumerable<PlanMealItemResponse>>> GetMealItemsForMealPlanAsync(Guid userId, Guid planMealId);

    Task<Result<PlanMealItemResponse>> UpdateMealItemInPlanAsync(Guid userId, Guid planMealId, Guid mealItemId, UpdatePlanMealItemRequest mealItemPlan);
    Task<Result<bool>> RemoveMealItemFromPlanAsync(Guid userId, Guid mealItemPlanId);

}