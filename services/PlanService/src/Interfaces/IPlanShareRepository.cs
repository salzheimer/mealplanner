using PlanService.Models;

namespace PlanService.Interfaces;
public interface IPlanShareRepository
{   
    Task<PlanShare> CreatePlanShareAsync(PlanShare planShare);
    Task<IEnumerable<PlanShare>> GetPlanSharesByPlanIdAsync(int planId);
    Task<IEnumerable<PlanShare>> GetPlanSharesBySharedByUserIdAsync(int userId);
    Task<PlanShare?> GetPlanShareByIdAsync(int planShareId);
    Task<bool> UpdatePlanShareAsync(PlanShare planShare);
    Task<bool> DeletePlanShareAsync(int planShareId);
}