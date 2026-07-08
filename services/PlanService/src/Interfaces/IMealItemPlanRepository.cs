using  PlanService.Models;

namespace PlanService.Interfaces; 
public interface IMealItemPlanRepository
{
    Task<PlanMealItem?> GetByIdAsync(Guid mealItemPlanId);
    Task<PlanMealItem?> AddMealItemToMealPlanAsync(PlanMealItem mealItemPlan);
    Task<IEnumerable<PlanMealItem>> GetMealItemsForMealPlanAsync(Guid planId);
    Task<bool> RemoveMealItemFromMealPlanAsync(Guid mealItemPlanId);
    Task<bool> UpdateMealItemInMealPlanAsync(PlanMealItem mealItemPlan);
}
