using PlanService.Models;
namespace PlanService.Repositories;

public interface IPlanRepository
{
    Task<IEnumerable<Plan>> GetAllGroupPlansAsync(int groupId);
    Task<Plan?> GetPlanByIdAsync(int id);
    Task AddPlanAsync(Plan plan);
    Task UpdatePlanAsync(Plan plan);
    Task DeletePlanAsync(int id);
}