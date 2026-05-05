
using PlanService.Interfaces;
using Shared.Models;
using PlanService.Models;
using PlanService.Mappings;

namespace PlanService.Services;

public class PlanningService : IPlanningService
{
    private readonly IPlanRepository _planRepository;
    private readonly IPlanShareRepository _planShareRepository;

    public PlanningService(IPlanRepository planRepository, IPlanShareRepository planShareRepository)
    {
        _planRepository = planRepository;
        _planShareRepository = planShareRepository;
    }

    #region Plan Methods

    public async Task<Result<PlanSummaryDto>> CreatePlanAsync(int currentUserId, PlanCreateDto plan)
    {
        var newPlan = new Plan
        {
            OwnerUserId = currentUserId,
            Name = plan.Name,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId,
            UpdatedBy = currentUserId
        };

        var createdPlan = await _planRepository.CreatePlanAsync(newPlan);
        if (createdPlan == null)
        {
            return Result<PlanSummaryDto>.Failure(PlanningErrors.UnableToCreate);
        }
        var planDto = new PlanSummaryDto
        (
             Id: createdPlan.Id,
             Name: createdPlan.Name,
             OwnerUserId: createdPlan.OwnerUserId,
             StartDate: createdPlan.StartDate,
             EndDate: createdPlan.EndDate

        );

        return Result<PlanSummaryDto>.Success(planDto);
    }

    public async Task<Result<PlanSummaryDto>> GetPlanByIdAsync(int id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return Result<PlanSummaryDto>.Failure(PlanningErrors.PlanNotFound);
        }

        var planDto = new PlanSummaryDto
        (
            Id: plan.Id,
            Name: plan.Name,
            OwnerUserId: plan.OwnerUserId,
            StartDate: plan.StartDate,
            EndDate: plan.EndDate
        );

        return Result<PlanSummaryDto>.Success(planDto);
    }

    public async Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByDateRangeAsync(int currentUserId, DateTime startDate, DateTime endDate)
    {
        IEnumerable<PlanSummaryDto> planDtos = new List<PlanSummaryDto>();
        //todo: check what plans the user has access to (owned/shared) and filter by date range in repository
        var plans = await _planRepository.GetPlansByDateRangeAsync(startDate, endDate);
        planDtos = await UserPlans(currentUserId, planDtos, plans);
        return Result<IEnumerable<PlanSummaryDto>>.Success(planDtos);
    }

    public async Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByEndDateAsync(int currentUserId, DateTime endDate)
    {
        IEnumerable<PlanSummaryDto> planDtos = new List<PlanSummaryDto>();
        //todo: check what plans the user has access to (owned/shared) and filter by end date in repository
        var plans = await _planRepository.GetPlansByEndDateAsync(endDate);
        planDtos = await UserPlans(currentUserId, planDtos, plans);

        return Result<IEnumerable<PlanSummaryDto>>.Success(planDtos);
    }

    private async Task<IEnumerable<PlanSummaryDto>> UserPlans(int currentUserId, IEnumerable<PlanSummaryDto> planDtos, IEnumerable<Plan> plans)
    {
        foreach (var plan in plans)
        {
            if (await UserHasAccessToPlan(currentUserId, plan.Id))
            {
                planDtos = planDtos.Append(new PlanSummaryDto
                (
                    Id: plan.Id,
                    Name: plan.Name,
                    OwnerUserId: plan.OwnerUserId,
                    StartDate: plan.StartDate,
                    EndDate: plan.EndDate
                ));
            }

        }

        return planDtos;
    }

    public async Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByStartDateAsync(int currentUserId, DateTime startDate)
    {
        IEnumerable<PlanSummaryDto> planDtos = new List<PlanSummaryDto>();
        //todo: check what plans the user has access to (owned/shared) and filter by start date in repository
        var plans = await _planRepository.GetPlansByStartDateAsync(startDate);
        planDtos = await UserPlans(currentUserId, planDtos, plans);
        return Result<IEnumerable<PlanSummaryDto>>.Success(planDtos);
    }

    public async Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansForUserAsync(int userId)
    {
        var plans = await _planRepository.GetPlansByOwnerAsync(userId);
        var sharedPlans = await _planShareRepository.GetPlanSharesBySharedWithUserIdAsync(userId);

        foreach (var share in sharedPlans)
        {
            var sharedPlan = await _planRepository.GetPlanByIdAsync(share.PlanId);
            if (sharedPlan != null)
            {
                plans.Append(sharedPlan);
            }
        }

        var planDtos = plans.Select(plan => new PlanSummaryDto
        (
            Id: plan.Id,
            Name: plan.Name,
            OwnerUserId: plan.OwnerUserId,
            StartDate: plan.StartDate,
            EndDate: plan.EndDate
        ));
        return Result<IEnumerable<PlanSummaryDto>>.Success(planDtos);
    }

    public async Task<Result<PlanSummaryDto>> UpdatePlanAsync(int currentUserId, PlanUpdateDto plan)
    {
        //todo: check if user has permission to update the plan (owner/shared with edit permission)
        if (!await UserHasAccessToPlan(currentUserId, plan.Id))
        {
            return Result<PlanSummaryDto>.Failure(PlanningErrors.Unauthorized);
        }

        var existingPlan = await _planRepository.GetPlanByIdAsync(plan.Id);
        if (existingPlan == null)
        {
            return Result<PlanSummaryDto>.Failure(PlanningErrors.PlanNotFound);
        }

        existingPlan.Name = plan.Name;
        existingPlan.StartDate = plan.StartDate;
        existingPlan.EndDate = plan.EndDate;

        var updated = await _planRepository.UpdatePlanAsync(existingPlan);
        if (!updated)
        {
            return Result<PlanSummaryDto>.Failure(PlanningErrors.UnableToUpdate);
        }

        return Result<PlanSummaryDto>.Success(new PlanSummaryDto
        (
            Id: existingPlan.Id,
            Name: existingPlan.Name,
            OwnerUserId: existingPlan.OwnerUserId,
            StartDate: existingPlan.StartDate,
            EndDate: existingPlan.EndDate
        ));
    }

    public async Task<Result<bool>> DeletePlanAsync(int currentUserId, int id)
    {
        //todo: check if user has permission to delete the plan (owner/shared with edit permission)

        var plan = await _planRepository.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return Result<bool>.Failure(PlanningErrors.PlanNotFound);
        }
        if (plan.OwnerUserId != currentUserId)
        {
            return Result<bool>.Failure(PlanningErrors.Unauthorized);
        }

        var deleted = await _planRepository.DeletePlanAsync(id);
        if (!deleted)
        {
            return Result<bool>.Failure(PlanningErrors.UnableToDelete);
        }

        return Result<bool>.Success(true);
    }


    #endregion

    #region PlanShare Methods

    public async Task<Result<PlanShareDto>> CreatePlanShareAsync(int currentUserId, PlanShareCreateDto planShare)
    {

        var newPlanShare = new PlanShare
        {
            PlanId = planShare.PlanId,
            SharedWithUserId = planShare.SharedWithUserId,
            SharedWithGroupId = planShare.SharedWithGroupId,
            SharedByUserId = planShare.SharedByUserId,
            Permission = EnumMappings.ToEntityPermission(planShare.Permission),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = planShare.ExpiresAt
        };

        var createdPlanShare = await _planShareRepository.CreatePlanShareAsync(newPlanShare);
        if (createdPlanShare == null)
        {
            return Result<PlanShareDto>.Failure(PlanningErrors.UnableToCreate);
        }
        var planShareDto = new PlanShareDto
        (
            Id: createdPlanShare.Id,
            PlanId: createdPlanShare.PlanId,
            SharedWithUserId: createdPlanShare.SharedWithUserId,
            SharedWithGroupId: createdPlanShare.SharedWithGroupId,
            SharedByUserId: createdPlanShare.SharedByUserId,
            Permission: createdPlanShare.Permission.ToDtoPermission(),
            CreatedAt: createdPlanShare.CreatedAt,
            ExpiresAt: createdPlanShare.ExpiresAt
        );

        return Result<PlanShareDto>.Success(planShareDto);
    }

    public async Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesByPlanIdAsync(int currentUserId, int planId)
    {
        //todo: check if user has permission to view the plan shares (owner/shared with view permission)
        if (!await UserHasAccessToPlan(currentUserId, planId))
        {
            return Result<IEnumerable<PlanShareDto>>.Failure(PlanningErrors.Unauthorized);
        }

        var planShares = await _planShareRepository.GetPlanSharesByPlanIdAsync(planId);
        var planShareDtos = planShares.Select(planShare => new PlanShareDto
        (
            Id: planShare.Id,
            PlanId: planShare.PlanId,
            SharedByUserId: planShare.SharedByUserId,
            SharedWithUserId: planShare.SharedWithUserId,
            SharedWithGroupId: planShare.SharedWithGroupId,
            Permission: planShare.Permission.ToDtoPermission(),
            CreatedAt: planShare.CreatedAt,
            ExpiresAt: planShare.ExpiresAt
        ));
        return Result<IEnumerable<PlanShareDto>>.Success(planShareDtos);
    }

    public async Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesBySharedByUserIdAsync(int currentUserId)
    {
        var planShares = await _planShareRepository.GetPlanSharesBySharedByUserIdAsync(currentUserId);
        var planShareDtos = planShares.Select(planShare => new PlanShareDto
        (
            Id: planShare.Id,
            PlanId: planShare.PlanId,
            SharedByUserId: planShare.SharedByUserId,
            SharedWithUserId: planShare.SharedWithUserId,
            SharedWithGroupId: planShare.SharedWithGroupId,
            Permission: planShare.Permission.ToDtoPermission(),
            CreatedAt: planShare.CreatedAt,
            ExpiresAt: planShare.ExpiresAt

        ));
        return Result<IEnumerable<PlanShareDto>>.Success(planShareDtos);
    }

    public async Task<Result<PlanShareDto>> UpdatePlanShareAsync(int currentUserId, PlanShareUpdateDto planShare)
    {
        //todo: check if user has permission to update the plan share (owner/shared with edit permission)
        if (!await UserHasAccessToPlan(currentUserId, planShare.PlanId))
        {
            return Result<PlanShareDto>.Failure(PlanningErrors.Unauthorized);
        }

        var existingPlanShare = await _planShareRepository.GetPlanShareByIdAsync(planShare.Id);
        if (existingPlanShare == null)
        {
            return Result<PlanShareDto>.Failure(PlanningErrors.PlanShareNotFound);
        }

        existingPlanShare.Id = planShare.PlanId;
        existingPlanShare.SharedWithUserId = planShare.SharedWithUserId;
        existingPlanShare.SharedWithGroupId = planShare.SharedWithGroupId;
        existingPlanShare.SharedByUserId = planShare.SharedByUserId;
        existingPlanShare.Permission = EnumMappings.ToEntityPermission(planShare.Permission);
        existingPlanShare.ExpiresAt = planShare.ExpiresAt;

        var updated = await _planShareRepository.UpdatePlanShareAsync(existingPlanShare);
        if (!updated)
        {
            return Result<PlanShareDto>.Failure(PlanningErrors.UnableToUpdate);
        }

        return Result<PlanShareDto>.Success(new PlanShareDto
        (
            Id: existingPlanShare.Id,
            PlanId: existingPlanShare.PlanId,
            SharedByUserId: existingPlanShare.SharedByUserId,
            SharedWithUserId: existingPlanShare.SharedWithUserId,
            SharedWithGroupId: existingPlanShare.SharedWithGroupId,
            Permission: existingPlanShare.Permission.ToDtoPermission(),
            CreatedAt: existingPlanShare.CreatedAt,
            ExpiresAt: existingPlanShare.ExpiresAt
        ));
    }


    public async Task<Result<bool>> DeletePlanShareAsync(int currentUserId, int planShareId)
    {
        //todo: check if user has permission to delete the plan share (owner/shared with edit permission)
        var planShare = await _planShareRepository.GetPlanSharesByPlanIdAsync(planShareId);
        if (planShare == null)
        {
            return Result<bool>.Failure(PlanningErrors.PlanShareNotFound);
        }

        var deleted = await _planShareRepository.DeletePlanShareAsync(planShareId);
        if (!deleted)
        {
            return Result<bool>.Failure(PlanningErrors.UnableToDelete);
        }

        return Result<bool>.Success(true);
    }
    #endregion

    #region private helper methods

    private async Task<bool> UserHasAccessToPlan(int userId, int planId)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planId);
        if (plan == null)
        {
            return false;
        }
        if (plan.OwnerUserId == userId)
        {
            return true;
        }
        //check if plan is shared with user directly
        var shares = await _planShareRepository.GetPlanSharesByPlanIdAsync(planId);
        if (shares.Any(share => share.SharedWithUserId == userId))
        {
            return true;
        }
        return false;
    }

    #endregion



}