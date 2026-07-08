using  PlanService.Models;

namespace PlanService.Interfaces;

public interface IPlanMealItemStatusTypeRepository
{
    Task<MealItemPlanStatusType?> GetByIdAsync(int planMealItemStatus);
    Task<MealItemPlanStatusType?> GetByNameAsync(string planMealItemStatusName);
    Task<IEnumerable<MealItemPlanStatusType>> GetAllAsync();
    Task<MealItemPlanStatusType?> CreateAsync(MealItemPlanStatusType statusType);
    Task<bool> UpdateAsync(MealItemPlanStatusType statusType);
}