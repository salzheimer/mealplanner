using PlanService.Models;

namespace PlanService.Interfaces;
public interface IPlanRepository
{
    Task<Plan?> CreatePlanAsync(Plan plan);
    Task<Plan?> GetPlanByIdAsync(Guid id);
    Task<IEnumerable<Plan>> GetPlansByOwnerAsync(Guid userId);
    Task<IEnumerable<Plan>> GetPlansByStartDateAsync(DateTime startDate);
    Task<IEnumerable<Plan>> GetPlansByEndDateAsync(DateTime endDate);
    Task<IEnumerable<Plan>> GetPlansByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Plan>> GetByIdsAsync(HashSet<Guid> sharedPlanIds);
    Task<bool> UpdatePlanAsync(Plan plan);
    Task<bool> DeletePlanAsync(Guid id);
}