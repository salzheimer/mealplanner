
using PlanService.Interfaces;
using PlanService.Clients;
using Shared.Models;
using PlanService.Models;
using PlanService.Mappings;

namespace PlanService.Services;

public class PlanningService : IPlanningService
{
    private readonly IPlanRepository _planRepository;
    private readonly IPlanShareRepository _planShareRepository;
    private readonly IIdentityServiceClient _identityClient;

    public PlanningService(IPlanRepository planRepository, IPlanShareRepository planShareRepository, IIdentityServiceClient identityClient)
    {
        _planRepository = planRepository;
        _planShareRepository = planShareRepository;
        _identityClient = identityClient;
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
            return Result<PlanSummaryDto>.Failure(PlanningErrors.UnableToCreate);

        return Result<PlanSummaryDto>.Success(ToDto(createdPlan));
    }

    public async Task<Result<PlanSummaryDto>> GetPlanByIdAsync(int currentUserId, int id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id);
        if (plan == null)
            return Result<PlanSummaryDto>.Failure(PlanningErrors.PlanNotFound);
        if (!await UserHasAccessToPlan(currentUserId, id))
            return Result<PlanSummaryDto>.Failure(PlanningErrors.Unauthorized);

        return Result<PlanSummaryDto>.Success(ToDto(plan));
    }

    public async Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByDateRangeAsync(int currentUserId, DateTime startDate, DateTime endDate)
    {
        var plans = await _planRepository.GetPlansByDateRangeAsync(startDate, endDate);
        return Result<IEnumerable<PlanSummaryDto>>.Success(await FilterToUserPlans(currentUserId, plans));
    }

    public async Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByEndDateAsync(int currentUserId, DateTime endDate)
    {
        var plans = await _planRepository.GetPlansByEndDateAsync(endDate);
        return Result<IEnumerable<PlanSummaryDto>>.Success(await FilterToUserPlans(currentUserId, plans));
    }

    public async Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansByStartDateAsync(int currentUserId, DateTime startDate)
    {
        var plans = await _planRepository.GetPlansByStartDateAsync(startDate);
        return Result<IEnumerable<PlanSummaryDto>>.Success(await FilterToUserPlans(currentUserId, plans));
    }

    public async Task<Result<IEnumerable<PlanSummaryDto>>> GetPlansForUserAsync(int userId)
    {
        var ownedPlans = await _planRepository.GetPlansByOwnerAsync(userId);

        var permissions = await _identityClient.GetPermissionsForResourceAsync(ResourceType.Plan, 0);
        // Shared plans are included via the IdentityService; owned plans cover the base case
        // TODO: wire subject-scoped permissions endpoint to include plans shared with userId

        var planDtos = ownedPlans.Select(ToDto);
        return Result<IEnumerable<PlanSummaryDto>>.Success(planDtos);
    }

    public async Task<Result<PlanSummaryDto>> UpdatePlanAsync(int currentUserId, PlanUpdateDto plan)
    {
        if (!await UserHasEditAccessToPlan(currentUserId, plan.Id))
            return Result<PlanSummaryDto>.Failure(PlanningErrors.Unauthorized);

        var existingPlan = await _planRepository.GetPlanByIdAsync(plan.Id);
        if (existingPlan == null)
            return Result<PlanSummaryDto>.Failure(PlanningErrors.PlanNotFound);

        existingPlan.Name = plan.Name;
        existingPlan.StartDate = plan.StartDate;
        existingPlan.EndDate = plan.EndDate;

        var updated = await _planRepository.UpdatePlanAsync(existingPlan);
        if (!updated)
            return Result<PlanSummaryDto>.Failure(PlanningErrors.UnableToUpdate);

        return Result<PlanSummaryDto>.Success(ToDto(existingPlan));
    }

    public async Task<Result<bool>> DeletePlanAsync(int currentUserId, int id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id);
        if (plan == null)
            return Result<bool>.Failure(PlanningErrors.PlanNotFound);
        if (!await UserHasEditAccessToPlan(currentUserId, id))
            return Result<bool>.Failure(PlanningErrors.Unauthorized);

        var deleted = await _planRepository.DeletePlanAsync(id);
        if (!deleted)
            return Result<bool>.Failure(PlanningErrors.UnableToDelete);

        return Result<bool>.Success(true);
    }

    #endregion

    #region PlanShare Methods

    public async Task<Result<PlanShareDto>> CreatePlanShareAsync(int currentUserId, PlanShareCreateDto planShare)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planShare.PlanId);
        if (plan == null)
            return Result<PlanShareDto>.Failure(PlanningErrors.PlanNotFound);
        if (plan.OwnerUserId != currentUserId)
            return Result<PlanShareDto>.Failure(PlanningErrors.Unauthorized);

        var newPlanShare = new PlanShare
        {
            PlanId = planShare.PlanId,
            SharedWithUserId = planShare.SharedWithUserId,
            SharedWithGroupId = planShare.SharedWithGroupId,
            SharedByUserId = currentUserId,
            Permission = EnumMappings.ToEntityPermission(planShare.Permission),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = planShare.ExpiresAt
        };

        var createdPlanShare = await _planShareRepository.CreatePlanShareAsync(newPlanShare);
        if (createdPlanShare == null)
            return Result<PlanShareDto>.Failure(PlanningErrors.UnableToCreate);

        return Result<PlanShareDto>.Success(ToShareDto(createdPlanShare));
    }

    public async Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesByPlanIdAsync(int currentUserId, int planId)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planId);
        if (plan == null)
            return Result<IEnumerable<PlanShareDto>>.Failure(PlanningErrors.PlanNotFound);
        if (plan.OwnerUserId != currentUserId)
            return Result<IEnumerable<PlanShareDto>>.Failure(PlanningErrors.Unauthorized);

        var planShares = await _planShareRepository.GetPlanSharesByPlanIdAsync(planId);
        return Result<IEnumerable<PlanShareDto>>.Success(planShares.Select(ToShareDto));
    }

    public async Task<Result<IEnumerable<PlanShareDto>>> GetPlanSharesBySharedByUserIdAsync(int currentUserId)
    {
        var planShares = await _planShareRepository.GetPlanSharesBySharedByUserIdAsync(currentUserId);
        return Result<IEnumerable<PlanShareDto>>.Success(planShares.Select(ToShareDto));
    }

    public async Task<Result<PlanShareDto>> UpdatePlanShareAsync(int currentUserId, PlanShareUpdateDto planShare)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planShare.PlanId);
        if (plan == null)
            return Result<PlanShareDto>.Failure(PlanningErrors.PlanNotFound);
        if (plan.OwnerUserId != currentUserId)
            return Result<PlanShareDto>.Failure(PlanningErrors.Unauthorized);

        var existingPlanShare = await _planShareRepository.GetPlanShareByIdAsync(planShare.Id);
        if (existingPlanShare == null)
            return Result<PlanShareDto>.Failure(PlanningErrors.PlanShareNotFound);

        existingPlanShare.SharedWithUserId = planShare.SharedWithUserId;
        existingPlanShare.SharedWithGroupId = planShare.SharedWithGroupId;
        existingPlanShare.Permission = EnumMappings.ToEntityPermission(planShare.Permission);
        existingPlanShare.ExpiresAt = planShare.ExpiresAt;

        var updated = await _planShareRepository.UpdatePlanShareAsync(existingPlanShare);
        if (!updated)
            return Result<PlanShareDto>.Failure(PlanningErrors.UnableToUpdate);

        return Result<PlanShareDto>.Success(ToShareDto(existingPlanShare));
    }

    public async Task<Result<bool>> DeletePlanShareAsync(int currentUserId, int planShareId)
    {
        var planShare = await _planShareRepository.GetPlanShareByIdAsync(planShareId);
        if (planShare == null)
            return Result<bool>.Failure(PlanningErrors.PlanShareNotFound);

        var plan = await _planRepository.GetPlanByIdAsync(planShare.PlanId);
        if (plan == null)
            return Result<bool>.Failure(PlanningErrors.PlanNotFound);
        if (plan.OwnerUserId != currentUserId)
            return Result<bool>.Failure(PlanningErrors.Unauthorized);

        var deleted = await _planShareRepository.DeletePlanShareAsync(planShareId);
        if (!deleted)
            return Result<bool>.Failure(PlanningErrors.UnableToDelete);

        return Result<bool>.Success(true);
    }

    #endregion

    #region private helper methods

    private async Task<bool> UserHasAccessToPlan(int userId, int planId)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planId);
        if (plan == null) return false;
        if (plan.OwnerUserId == userId) return true;

        var permissions = await _identityClient.GetPermissionsForResourceAsync(ResourceType.Plan, planId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId);
    }

    private async Task<bool> UserHasEditAccessToPlan(int userId, int planId)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planId);
        if (plan == null) return false;
        if (plan.OwnerUserId == userId) return true;

        var permissions = await _identityClient.GetPermissionsForResourceAsync(ResourceType.Plan, planId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId && p.Permission >= Shared.Models.Permission.Edit);
    }

    private async Task<IEnumerable<PlanSummaryDto>> FilterToUserPlans(int userId, IEnumerable<Plan> plans)
    {
        var result = new List<PlanSummaryDto>();
        foreach (var plan in plans)
        {
            if (await UserHasAccessToPlan(userId, plan.Id))
                result.Add(ToDto(plan));
        }
        return result;
    }

    private static PlanSummaryDto ToDto(Plan plan) => new(
        Id: plan.Id,
        Name: plan.Name,
        OwnerUserId: plan.OwnerUserId,
        StartDate: plan.StartDate,
        EndDate: plan.EndDate
    );

    private static PlanShareDto ToShareDto(PlanShare ps) => new(
        Id: ps.Id,
        PlanId: ps.PlanId,
        SharedByUserId: ps.SharedByUserId,
        SharedWithUserId: ps.SharedWithUserId,
        SharedWithGroupId: ps.SharedWithGroupId,
        Permission: ps.Permission.ToDtoPermission(),
        CreatedAt: ps.CreatedAt,
        ExpiresAt: ps.ExpiresAt
    );

    #endregion
}
