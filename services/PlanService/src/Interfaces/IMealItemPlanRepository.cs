using  PlanService.Models;

namespace PlanService.Interfaces; 
public interface IMealItemPlanRepository
{
    Task<MealItemPlan?> AddMealItemToMealPlanAsync(MealItemPlan mealItemPlan);
    Task<IEnumerable<MealItemPlan>> GetMealItemsForMealPlanAsync(int planId);
    Task<bool> RemoveMealItemFromMealPlanAsync(int mealItemPlanId);
     Task<bool> UpdateMealItemInMealPlanAsync(MealItemPlan mealItemPlan);
}