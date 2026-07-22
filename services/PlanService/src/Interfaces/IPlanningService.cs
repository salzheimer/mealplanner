
using System.Data;
using PlanService.Contracts;
using Shared.Models;

namespace PlanService.Interfaces;
public interface IPlanningService
{
    Task<Result<PlanSummaryResponse>> CreatePlanAsync(Guid currentUserId, CreatePlanRequest plan);
    Task<Result<PlanSummaryResponse>> GetPlanByIdAsync(Guid currentUserId, Guid id);
    Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansForUserAsync(Guid userId);
    Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansByStartDateAsync(Guid currentUserId, DateTime startDate);
    Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansByEndDateAsync(Guid currentUserId, DateTime endDate);
    Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansByDateRangeAsync(Guid currentUserId, DateTime startDate, DateTime endDate);
    Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansSharedWithMeAsync(Guid userId);
    Task<Result<PlanSummaryResponse>> UpdatePlanAsync(Guid currentUserId, Guid plandId,UpdatePlanRequest plan);
    Task<Result<bool>> DeletePlanAsync(Guid currentUserId, Guid id);

    Task<Result<SharePlanResponse>> SharePlanAsync(Guid currentUserId, SharePlanRequest planShare);
   
}
