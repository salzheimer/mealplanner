using PlanService.Models;

namespace PlanService.Interfaces;

public interface IMealPlanRepository
{
    Task<MealPlan> CreateMealPlanAsync(MealPlan mealPlan);
    Task<MealPlan?> GetMealPlanByIdAsync(int id);
    Task<IEnumerable<MealPlan?>> GetMealPlansByPlanIdAsync(int planId);
    Task<IEnumerable<MealPlan>> GetMealPlansForUserAsync(int userId);
    Task<IEnumerable<MealPlan>> GetMealPlansByStartDateAsync(DateTime startDate);
    Task<IEnumerable<MealPlan>> GetMealPlansByEndDateAsync(DateTime endDate);
    Task<IEnumerable<MealPlan>> GetMealPlansByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> UpdateMealPlanAsync(MealPlan mealPlan);
    Task<bool> DeleteMealPlanAsync(int id);
}