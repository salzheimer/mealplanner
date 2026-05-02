
using Shared.Models;

namespace PlanService.Interfaces;
public interface IPlanningService
{
    Task<Result<PlanDto>> CreatePlanAsync(PlanCreateDto plan);
    Task<Result<PlanDto>> GetPlanByIdAsync(int id);
    Task<Result<IEnumerable<PlanDto>>> GetPlansForUserAsync(int userId);
    Task<Result<IEnumerable<PlanDto>>> GetPlansByStartDateAsync(DateTime startDate);
    Task<Result<IEnumerable<PlanDto>>> GetPlansByEndDateAsync(DateTime endDate);
    Task<Result<IEnumerable<PlanDto>>> GetPlansByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Result<PlanDto>> UpdatePlanAsync(PlanUpdateDto plan);
    Task<Result<bool>> DeletePlanAsync(int id);

    Task<Result<PlanShareDto>> CreatePlanShareAsync(PlanShareCreateDto planShare);
    Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesByPlanIdAsync(int planId);
    Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesByUserIdAsync(int userId);
    Task<Result<PlanShareDto>> UpdatePlanShareAsync(PlanShareUpdateDto planShare);
    Task<Result<bool>> DeletePlanShareAsync(int planShareId);
}