
using System.Data;
using Shared.Models;

namespace PlanService.Interfaces;
public interface IPlanningService
{
    Task<Result<PlanSummaryDto>> CreatePlanAsync(int currentUserId, PlanCreateDto plan);
    Task<Result<PlanSummaryDto>> GetPlanByIdAsync(int currentUserId, int id);
    Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansForUserAsync(int userId);
    Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByStartDateAsync(int currentUserId, DateTime startDate);
    Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByEndDateAsync(int currentUserId, DateTime endDate);
    Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByDateRangeAsync(int currentUserId, DateTime startDate, DateTime endDate);
    Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansSharedWithMeAsync(int userId);
    Task<Result<PlanSummaryDto>> UpdatePlanAsync(int currentUserId, int plandId,PlanUpdateDto plan);
    Task<Result<bool>> DeletePlanAsync(int currentUserId, int id);

    Task<Result<PlanShareDto>> CreatePlanShareAsync(int currentUserId, PlanShareCreateDto planShare);
    Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesByPlanIdAsync(int currentUserId, int planId);
    Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesBySharedByUserIdAsync(int currentUserId);
    Task<Result<PlanShareDto>> UpdatePlanShareAsync(int currentUserId, int shareId, PlanShareUpdateDto planShare);
    Task<Result<bool>> DeletePlanShareAsync(int currentUserId, int planShareId);
}
