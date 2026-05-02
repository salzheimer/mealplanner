using PlanService.Models;

namespace PlanService.Interfaces;
public interface IPlanRepository
{
    Task<Plan?> CreatePlanAsync(Plan plan);
    Task<Plan?> GetPlanByIdAsync(int id);
    Task<IEnumerable<Plan>> GetPlansForUserAsync(int userId);
    Task<IEnumerable<Plan>> GetPlansByStartDateAsync(DateTime startDate);
    Task<IEnumerable<Plan>> GetPlansByEndDateAsync(DateTime endDate);
    Task<IEnumerable<Plan>> GetPlansByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> UpdatePlanAsync(Plan plan);
    Task<bool> DeletePlanAsync(int id);
}