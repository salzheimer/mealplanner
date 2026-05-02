
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

    public async Task<Result<PlanDto>> CreatePlanAsync(PlanCreateDto plan)
    {
        var newPlan = new Plan
        {
            OwnerUserId = plan.OwnerUserId,
            Name = plan.Name,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate
        };

        var createdPlan = await _planRepository.CreatePlanAsync(newPlan);
        if (createdPlan == null)
        {
            return Result<PlanDto>.Failure(PlanningErrors.UnableToCreate);
        }
        var planDto = new PlanDto
        (
             Id: createdPlan.Id,
             Name: createdPlan.Name,
             OwnerUserId: createdPlan.OwnerUserId,
             StartDate: createdPlan.StartDate,
             EndDate: createdPlan.EndDate
        );

        return Result<PlanDto>.Success(planDto);
    }

    public async Task<Result<PlanDto>> GetPlanByIdAsync(int id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return Result<PlanDto>.Failure(PlanningErrors.PlanNotFound);
        }

        var planDto = new PlanDto
        (
             Id: plan.Id,
             Name: plan.Name,
             OwnerUserId: plan.OwnerUserId,
             StartDate: plan.StartDate,
             EndDate: plan.EndDate
        );

        return Result<PlanDto>.Success(planDto);
    }

    public async Task<Result<IEnumerable<PlanDto>>> GetPlansByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var plans = await _planRepository.GetPlansByDateRangeAsync(startDate, endDate);
        var planDtos = plans.Select(plan => new PlanDto
        (
            Id: plan.Id,
            Name: plan.Name,
            OwnerUserId: plan.OwnerUserId,
            StartDate: plan.StartDate,
            EndDate: plan.EndDate
        ));
        return Result<IEnumerable<PlanDto>>.Success(planDtos);
    }

    public async Task<Result<IEnumerable<PlanDto>>> GetPlansByEndDateAsync(DateTime endDate)
    {
        var plans = await _planRepository.GetPlansByEndDateAsync(endDate);
        var planDtos = plans.Select(plan => new PlanDto
        (
            Id: plan.Id,
            Name: plan.Name,
            OwnerUserId: plan.OwnerUserId,
            StartDate: plan.StartDate,
            EndDate: plan.EndDate
        ));
        return Result<IEnumerable<PlanDto>>.Success(planDtos);
    }

    public async Task<Result<IEnumerable<PlanDto>>> GetPlansByStartDateAsync(DateTime startDate)
    {
        var plans = await _planRepository.GetPlansByStartDateAsync(startDate);
        var planDtos = plans.Select(plan => new PlanDto
        (
            Id: plan.Id,
            Name: plan.Name,
            OwnerUserId: plan.OwnerUserId,
            StartDate: plan.StartDate,
            EndDate: plan.EndDate
        ));
        return Result<IEnumerable<PlanDto>>.Success(planDtos);
    }

    public async Task<Result<IEnumerable<PlanDto>>> GetPlansForUserAsync(int userId)
    {
        var plans = await _planRepository.GetPlansForUserAsync(userId);
        var planDtos = plans.Select(plan => new PlanDto
        (
            Id: plan.Id,
            Name: plan.Name,
            OwnerUserId: plan.OwnerUserId,
            StartDate: plan.StartDate,
            EndDate: plan.EndDate
        ));
        return Result<IEnumerable<PlanDto>>.Success(planDtos);
    }

    public async Task<Result<PlanDto>> UpdatePlanAsync(PlanUpdateDto plan)
    {
        var existingPlan = await _planRepository.GetPlanByIdAsync(plan.Id);
        if (existingPlan == null)
        {
            return Result<PlanDto>.Failure(PlanningErrors.PlanNotFound);
        }

        existingPlan.Name = plan.Name;
        existingPlan.StartDate = plan.StartDate;
        existingPlan.EndDate = plan.EndDate;

        var updated = await _planRepository.UpdatePlanAsync(existingPlan);
        if (!updated)
        {
            return Result<PlanDto>.Failure(PlanningErrors.UnableToUpdate);
        }

        return Result<PlanDto>.Success(new PlanDto
        (
            Id: existingPlan.Id,
            Name: existingPlan.Name,
            OwnerUserId: existingPlan.OwnerUserId,
            StartDate: existingPlan.StartDate,
            EndDate: existingPlan.EndDate
        ));
    }

    public async Task<Result<bool>> DeletePlanAsync(int id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id);
        if (plan == null)
        {
            return Result<bool>.Failure(PlanningErrors.PlanNotFound);
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

    public async Task<Result<PlanShareDto>> CreatePlanShareAsync(PlanShareCreateDto planShare)
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

    public async Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesByPlanIdAsync(int planId)
    {
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

    public async Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesByUserIdAsync(int userId)
    {
        var planShares = await _planShareRepository.GetPlanSharesBySharedByUserIdAsync(userId);
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

    public async Task<Result<PlanShareDto>> UpdatePlanShareAsync(PlanShareUpdateDto planShare)
    {
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


    public async Task<Result<bool>> DeletePlanShareAsync(int planShareId)
    {
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





}