using PlanService.Models;

namespace PlanService.Interfaces;

public interface IMealPlanRepository
{
    Task<PlanMeal> CreateMealPlanAsync(PlanMeal mealPlan);
    Task<PlanMeal?> GetMealPlanByIdAsync(Guid id);
    Task<IEnumerable<PlanMeal?>> GetMealPlansByPlanIdAsync(Guid planId);
    Task<IEnumerable<PlanMeal>> GetMealPlansForUserAsync(Guid userId);
    Task<IEnumerable<PlanMeal>> GetMealPlansByStartDateAsync(DateTime startDate);
    Task<IEnumerable<PlanMeal>> GetMealPlansByEndDateAsync(DateTime endDate);
    Task<IEnumerable<PlanMeal>> GetMealPlansByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> UpdateMealPlanAsync(PlanMeal mealPlan);
    Task<bool> DeleteMealPlanAsync(Guid id);
}