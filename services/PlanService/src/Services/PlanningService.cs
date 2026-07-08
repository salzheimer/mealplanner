
using PlanService.Interfaces;
using PlanService.Contracts;
using Shared.Models;
using PlanService.Models;


namespace PlanService.Services;

public class PlanningService : IPlanningService
{
    private readonly IPlanRepository _planRepository;

    private readonly IAccessService _accessService;

    public PlanningService(IPlanRepository planRepository, IAccessService accessService)
    {
        _planRepository = planRepository;
        _accessService = accessService;
    }

    #region Plan Methods

    public async Task<Result<PlanSummaryResponse>> CreatePlanAsync(Guid currentUserId, CreatePlanRequest plan)
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
            return Result<PlanSummaryResponse>.Failure(PlanErrors.UnableToCreate);

        return Result<PlanSummaryResponse>.Success(ToDto(createdPlan));
    }

    public async Task<Result<PlanSummaryResponse>> GetPlanByIdAsync(Guid currentUserId, Guid id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id);
        if (plan == null)
            return Result<PlanSummaryResponse>.Failure(PlanErrors.PlanNotFound);
        if (!await UserHasAccessToPlan(currentUserId, id))
            return Result<PlanSummaryResponse>.Failure(PlanErrors.Unauthorized);

        return Result<PlanSummaryResponse>.Success(ToDto(plan));
    }

    public async Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansByDateRangeAsync(Guid currentUserId, DateTime startDate, DateTime endDate)
    {
        var plans = await _planRepository.GetPlansByDateRangeAsync(startDate, endDate);
        return Result<IEnumerable<PlanSummaryResponse>>.Success(await FilterToUserPlans(currentUserId, plans));
    }

    public async Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansByEndDateAsync(Guid currentUserId, DateTime endDate)
    {
        var plans = await _planRepository.GetPlansByEndDateAsync(endDate);
        return Result<IEnumerable<PlanSummaryResponse>>.Success(await FilterToUserPlans(currentUserId, plans));
    }

    public async Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansByStartDateAsync(Guid currentUserId, DateTime startDate)
    {
        var plans = await _planRepository.GetPlansByStartDateAsync(startDate);
        return Result<IEnumerable<PlanSummaryResponse>>.Success(await FilterToUserPlans(currentUserId, plans));
    }

    public async Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansForUserAsync(Guid userId)
    {
        var plans = await _planRepository.GetPlansByOwnerAsync(userId);

        var sharedPermissions = await _accessService.GetPlansSharedWithUser(userId);

        var sharePlanIds = sharedPermissions.IsSuccess
        ? sharedPermissions.Value!.Select(p => p.ResourceId).ToHashSet()
            : new HashSet<Guid>();


        var sharePlans = sharePlanIds.Any() ? await _planRepository.GetByIdsAsync(sharePlanIds) : Enumerable.Empty<Plan>();

        plans = plans.Concat(sharePlans);

        var planDtos = plans.Select(ToDto);
        return Result<IEnumerable<PlanSummaryResponse>>.Success(planDtos);
    }
    public async Task<Result<IEnumerable<PlanSummaryResponse>>> GetPlansSharedWithMeAsync(Guid userId)
    {

        //get plans shared with user
        var sharedPermissions = await _accessService.GetPlansSharedWithUser(userId);
        var sharedPlanIds = sharedPermissions.IsSuccess
            ? sharedPermissions.Value!.Select(p => p.ResourceId).ToHashSet()
            : new HashSet<Guid>();

        var sharedPlans = sharedPlanIds.Any() ? await _planRepository.GetByIdsAsync(sharedPlanIds) : Enumerable.Empty<Plan>();



        var planDtos = sharedPlans.Select(p => new PlanSummaryResponse(
            Id: p.PlanId,
            Name: p.Name,
            StartDate: p.StartDate,
            EndDate: p.EndDate,
            OwnerUserId: p.OwnerUserId
        ));
        return Result<IEnumerable<PlanSummaryResponse>>.Success(planDtos);
    }

    public async Task<Result<PlanSummaryResponse>> UpdatePlanAsync(Guid currentUserId, Guid planId, UpdatePlanRequest plan)
    {
        var existingPlan = await _planRepository.GetPlanByIdAsync(planId);
        if (existingPlan == null)
            return Result<PlanSummaryResponse>.Failure(PlanErrors.PlanNotFound);

        if (!await UserHasEditAccessToPlan(currentUserId, planId))
            return Result<PlanSummaryResponse>.Failure(PlanErrors.Unauthorized);


        existingPlan.Name = plan.Name;
        existingPlan.StartDate = plan.StartDate;
        existingPlan.EndDate = plan.EndDate;

        var updated = await _planRepository.UpdatePlanAsync(existingPlan);
        if (!updated)
            return Result<PlanSummaryResponse>.Failure(PlanErrors.UnableToUpdate);

        return Result<PlanSummaryResponse>.Success(ToDto(existingPlan));
    }

    public async Task<Result<bool>> DeletePlanAsync(Guid currentUserId, Guid id)
    {
        var plan = await _planRepository.GetPlanByIdAsync(id);
        if (plan == null)
            return Result<bool>.Failure(PlanErrors.PlanNotFound);
        if (!await UserHasEditAccessToPlan(currentUserId, id))
            return Result<bool>.Failure(PlanErrors.Unauthorized);

        var deleted = await _planRepository.DeletePlanAsync(id);
        if (!deleted)
            return Result<bool>.Failure(PlanErrors.UnableToDelete);

        return Result<bool>.Success(true);
    }

    #endregion

    #region PlanShare Methods

    public async Task<Result<SharePlanResponse>> SharePlanAsync(Guid currentUserId, SharePlanRequest planShareRequest)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planShareRequest.PlanId);
        if (plan == null)
            return Result<SharePlanResponse>.Failure(PlanErrors.PlanNotFound);
        if (plan.OwnerUserId != currentUserId)
            return Result<SharePlanResponse>.Failure(PlanErrors.Unauthorized);


        var permissionRequest = new CreateResourcePermissionRequest(
                "recipe",
                planShareRequest.PlanId,
                planShareRequest.SubjectTypeName,
                planShareRequest.SubjectId,
                planShareRequest.SubjectTypeName == "group" ? "view" : planShareRequest.PermissionTypeName,
                planShareRequest.GrantedBy,
                planShareRequest.ExpiresAt
                );
        var accessResponse = await _accessService.GrantAccessToResource(permissionRequest);
        if (accessResponse.IsSuccess == false)
            return Result<SharePlanResponse>.Failure(PlanErrors.UnableToShare);

         


        return Result<SharePlanResponse>.Success(ToShareDto(accessResponse.Value!));
    }

    

    

    #endregion

    #region private helper methods

    private async Task<bool> UserHasAccessToPlan(Guid userId, Guid planId)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planId);
        if (plan == null) return false;
        if (plan.OwnerUserId == userId) return true;

        var permissions = await _accessService.GetPlansSharedWithUser(userId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId);
    }

    private async Task<bool> UserHasEditAccessToPlan(Guid userId, Guid planId)
    {
        var plan = await _planRepository.GetPlanByIdAsync(planId);
        if (plan == null) return false;
        if (plan.OwnerUserId == userId) return true;

        var permissions = await _accessService.GetPlansSharedWithUser(userId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId && p.PermissionTypeName == "edit");
    }

    private async Task<IEnumerable<PlanSummaryResponse>> FilterToUserPlans(Guid userId, IEnumerable<Plan> plans)
    {
        var result = new List<PlanSummaryResponse>();
        foreach (var plan in plans)
        {
            if (await UserHasAccessToPlan(userId, plan.PlanId))
                result.Add(ToDto(plan));
        }
        return result;
    }

    private static PlanSummaryResponse ToDto(Plan plan) => new(
        Id: plan.PlanId,
        Name: plan.Name,
        OwnerUserId: plan.OwnerUserId,
        StartDate: plan.StartDate,
        EndDate: plan.EndDate
    );

    private static SharePlanResponse ToShareDto(ResourcePermissionResponse ps) => new(

        PlanId: ps.ResourceId,
        ResourceTypeName: ps.ResourceTypeName,
        ResourceTypeId: ps.ResourceTypeId,
        SubjectTypeName: ps.SubjectTypeName,
        SubjectTypeId: ps.SubjectTypeId,
        PermissionTypeName: ps.PermissionTypeName,
        PermissionTypeId: ps.PermissionTypeId,
        SubjectId: ps.SubjectId,
        GrantedBy: ps.GrantedBy,
        ExpiresAt: ps.ExpiresAt
    );

    #endregion
}
