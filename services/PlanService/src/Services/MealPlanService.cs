using PlanService.Interfaces;
using PlanService.Clients;
using PlanService.Mappings;
using PlanService.Models;
using Shared.Models;

namespace PlanService.Services;

public class MealPlanService : IMealPlanService
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IMealItemPlanRepository _mealItemPlanRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IIdentityServiceClient _identityClient;

    public MealPlanService(
        IMealPlanRepository mealPlanRepository,
        IMealItemPlanRepository mealItemPlanRepository,
        IPlanRepository planRepository,
        IIdentityServiceClient identityClient)
    {
        _mealPlanRepository = mealPlanRepository;
        _mealItemPlanRepository = mealItemPlanRepository;
        _planRepository = planRepository;
        _identityClient = identityClient;
    }

    #region MealPlan management

    public async Task<Result<MealPlanDto?>> CreateMealPlanAsync(int userId, MealPlanCreateDto mealPlan)
    {
        if (!await UserHasEditAccessToPlan(userId, mealPlan.PlanId))
            return Result<MealPlanDto?>.Failure(MealPlanErrors.Unauthorized);

        var plan = new MealPlan
        {
            MealId = mealPlan.MealId,
            PlanId = mealPlan.PlanId,
            ServeDate = mealPlan.ServeDate,
            EndDate = mealPlan.EndDate,
            AddedByUserId = userId
        };
        var createdPlan = await _mealPlanRepository.CreateMealPlanAsync(plan);
        if (createdPlan == null)
            return Result<MealPlanDto?>.Failure(MealPlanErrors.UnableToCreate);

        return Result<MealPlanDto?>.Success(ToDto(createdPlan));
    }

    public async Task<Result<MealPlanDto?>> GetMealPlanByIdAsync(int userId, int id)
    {
        var plan = await _mealPlanRepository.GetMealPlanByIdAsync(id);
        if (plan == null)
            return Result<MealPlanDto?>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasAccessToPlan(userId, plan.PlanId))
            return Result<MealPlanDto?>.Failure(MealPlanErrors.Unauthorized);

        return Result<MealPlanDto?>.Success(ToDto(plan));
    }

    public async Task<Result<MealPlanDto>> UpdateMealPlanAsync(int userId, MealPlanUpdateDto mealPlan)
    {
        var existingPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlan.Id);
        if (existingPlan == null)
            return Result<MealPlanDto>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasEditAccessToPlan(userId, existingPlan.PlanId))
            return Result<MealPlanDto>.Failure(MealPlanErrors.Unauthorized);

        existingPlan.MealId = mealPlan.MealId;
        existingPlan.ServeDate = mealPlan.ServeDate;
        existingPlan.EndDate = mealPlan.EndDate;

        var success = await _mealPlanRepository.UpdateMealPlanAsync(existingPlan);
        if (!success)
            return Result<MealPlanDto>.Failure(MealPlanErrors.UnableToUpdate);

        return Result<MealPlanDto>.Success(ToDto(existingPlan));
    }

    public async Task<Result<bool>> DeleteMealPlanAsync(int userId, int id)
    {
        var existingPlan = await _mealPlanRepository.GetMealPlanByIdAsync(id);
        if (existingPlan == null)
            return Result<bool>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasEditAccessToPlan(userId, existingPlan.PlanId))
            return Result<bool>.Failure(MealPlanErrors.Unauthorized);

        var success = await _mealPlanRepository.DeleteMealPlanAsync(id);
        if (!success)
            return Result<bool>.Failure(MealPlanErrors.UnableToDelete);

        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansForUserAsync(int userId)
    {
        var plans = await _mealPlanRepository.GetMealPlansForUserAsync(userId);
        return Result<IEnumerable<MealPlanDto>>.Success(plans.Select(ToDto).ToList());
    }

    public async Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByStartDateAsync(int userId, DateTime startDate)
    {
        var plans = await _mealPlanRepository.GetMealPlansByStartDateAsync(startDate);
        return Result<IEnumerable<MealPlanDto>>.Success(await FilterToUserMealPlans(userId, plans));
    }

    public async Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByEndDateAsync(int userId, DateTime endDate)
    {
        var plans = await _mealPlanRepository.GetMealPlansByEndDateAsync(endDate);
        return Result<IEnumerable<MealPlanDto>>.Success(await FilterToUserMealPlans(userId, plans));
    }

    public async Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var plans = await _mealPlanRepository.GetMealPlansByDateRangeAsync(startDate, endDate);
        return Result<IEnumerable<MealPlanDto>>.Success(await FilterToUserMealPlans(userId, plans));
    }

    #endregion

    #region MealItemPlan management

    public async Task<Result<MealItemPlanDto?>> AddMealItemToPlanAsync(int userId, MealItemPlanCreateDto mealItemPlan)
    {
        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealItemPlan.MealPlanId);
        if (mealPlan == null)
            return Result<MealItemPlanDto?>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasEditAccessToPlan(userId, mealPlan.PlanId))
            return Result<MealItemPlanDto?>.Failure(MealPlanErrors.Unauthorized);

        var mealItemPlanEntity = new MealItemPlan
        {
            MealPlanId = mealItemPlan.MealPlanId,
            MealItemId = mealItemPlan.MealItemId,
            AssignedToGuestName = mealItemPlan.AssignedToGuestName,
            AssignedToUserId = mealItemPlan.AssignedToUser,
            Status = EnumMappings.ToEntityItemStatus(mealItemPlan.Status ?? Shared.Models.ItemStatus.Pending),
            Notes = mealItemPlan.Notes?.ToString() ?? string.Empty
        };

        var created = await _mealItemPlanRepository.AddMealItemToMealPlanAsync(mealItemPlanEntity);
        if (created == null)
            return Result<MealItemPlanDto?>.Failure(MealPlanErrors.UnableToCreate);

        return Result<MealItemPlanDto?>.Success(ToItemDto(created));
    }

    public async Task<Result<IEnumerable<MealItemPlanDto>>> GetMealItemsForMealPlanAsync(int userId, int mealPlanId)
    {
        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlanId);
        if (mealPlan == null)
            return Result<IEnumerable<MealItemPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasAccessToPlan(userId, mealPlan.PlanId))
            return Result<IEnumerable<MealItemPlanDto>>.Failure(MealPlanErrors.Unauthorized);

        var items = await _mealItemPlanRepository.GetMealItemsForMealPlanAsync(mealPlanId);
        return Result<IEnumerable<MealItemPlanDto>>.Success(items.Select(ToItemDto).ToList());
    }

    public async Task<Result<MealItemPlanDto>> UpdateMealItemInPlanAsync(int userId, MealItemPlanUpdateDto mealItemPlan)
    {
        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealItemPlan.MealPlanId);
        if (mealPlan == null)
            return Result<MealItemPlanDto>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasEditAccessToPlan(userId, mealPlan.PlanId))
            return Result<MealItemPlanDto>.Failure(MealPlanErrors.Unauthorized);

        var items = await _mealItemPlanRepository.GetMealItemsForMealPlanAsync(mealItemPlan.MealPlanId);
        var entity = items.FirstOrDefault(mip => mip.Id == mealItemPlan.Id);
        if (entity == null)
            return Result<MealItemPlanDto>.Failure(MealPlanErrors.MealPlanNotFound);

        entity.MealItemId = mealItemPlan.MealItemId ?? entity.MealItemId;
        entity.AssignedToGuestName = mealItemPlan.AssignedToGuestName ?? entity.AssignedToGuestName;
        entity.AssignedToUserId = mealItemPlan.AssignedToUser ?? entity.AssignedToUserId;
        entity.Status = mealItemPlan.Status != null ? EnumMappings.ToEntityItemStatus(mealItemPlan.Status.Value) : entity.Status;
        entity.Notes = mealItemPlan.Notes ?? entity.Notes;

        var success = await _mealItemPlanRepository.UpdateMealItemInMealPlanAsync(entity);
        if (!success)
            return Result<MealItemPlanDto>.Failure(MealPlanErrors.UnableToUpdate);

        return Result<MealItemPlanDto>.Success(ToItemDto(entity));
    }

    public async Task<Result<bool>> RemoveMealItemFromPlanAsync(int userId, int mealItemPlanId)
    {
        var mealItemPlan = await _mealItemPlanRepository.GetByIdAsync(mealItemPlanId);
        if (mealItemPlan == null)
            return Result<bool>.Failure(MealPlanErrors.MealPlanNotFound);

        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealItemPlan.MealPlanId);
        if (mealPlan == null)
            return Result<bool>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasEditAccessToPlan(userId, mealPlan.PlanId))
            return Result<bool>.Failure(MealPlanErrors.Unauthorized);

        var success = await _mealItemPlanRepository.RemoveMealItemFromMealPlanAsync(mealItemPlanId);
        if (!success)
            return Result<bool>.Failure(MealPlanErrors.UnableToDelete);

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

    private async Task<IEnumerable<MealPlanDto>> FilterToUserMealPlans(int userId, IEnumerable<MealPlan> plans)
    {
        var result = new List<MealPlanDto>();
        var checkedPlanIds = new Dictionary<int, bool>();
        foreach (var mp in plans)
        {
            if (!checkedPlanIds.TryGetValue(mp.PlanId, out var hasAccess))
            {
                hasAccess = await UserHasAccessToPlan(userId, mp.PlanId);
                checkedPlanIds[mp.PlanId] = hasAccess;
            }
            if (hasAccess)
                result.Add(ToDto(mp));
        }
        return result;
    }

    private static MealPlanDto ToDto(MealPlan plan) => new(
        Id: plan.Id,
        MealId: plan.MealId,
        PlanId: plan.PlanId,
        ServeDate: plan.ServeDate,
        EndDate: plan.EndDate,
        AddedByUserId: plan.AddedByUserId,
        CreatedAt: plan.CreatedAt,
        UpdatedAt: plan.UpdatedAt
    );

    private static MealItemPlanDto ToItemDto(MealItemPlan mip) => new(
        Id: mip.Id,
        MealPlanId: mip.MealPlanId,
        MealItemId: mip.MealItemId,
        AssignedToGuestName: mip.AssignedToGuestName,
        AssignedToUser: mip.AssignedToUserId,
        Status: EnumMappings.ToDtoItemStatus(mip.Status),
        Notes: mip.Notes,
        CreatedAt: mip.CreatedAt,
        UpdatedAt: mip.UpdatedAt
    );

    #endregion
}
