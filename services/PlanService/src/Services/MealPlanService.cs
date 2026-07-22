using PlanService.Interfaces;
using PlanService.Contracts;
using PlanService.Models;
using Shared.Models;

namespace PlanService.Services;

public class MealPlanService : IMealPlanService
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IMealItemPlanRepository _mealItemPlanRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IAccessService _accessService;
    private readonly IPlanMealItemStatusTypeRepository _planMealItemStatusTypeRepository;

    public MealPlanService(
        IMealPlanRepository mealPlanRepository,
        IMealItemPlanRepository mealItemPlanRepository,
        IPlanRepository planRepository,
        IAccessService accessService,
        IPlanMealItemStatusTypeRepository planMealItemStatusTypeRepository
         )
    {
        _mealPlanRepository = mealPlanRepository;
        _mealItemPlanRepository = mealItemPlanRepository;
        _planRepository = planRepository;
        _accessService = accessService;
        _planMealItemStatusTypeRepository = planMealItemStatusTypeRepository;

    }

    #region MealPlan management

    public async Task<Result<PlanMealResponse?>> CreateMealPlanAsync(Guid userId, CreatePlanMealRequest mealPlan)
    {
        if (!await UserHasEditAccessToPlan(userId, mealPlan.PlanId))
            return Result<PlanMealResponse?>.Failure(MealPlanErrors.Unauthorized);

        var plan = new PlanMeal
        {
            MealId = mealPlan.MealId,
            PlanId = mealPlan.PlanId,
            ServeDate = mealPlan.ServeDate,
            EndDate = mealPlan.EndDate,
            AddedByUserId = userId
        };
        var createdPlan = await _mealPlanRepository.CreateMealPlanAsync(plan);
        if (createdPlan == null)
            return Result<PlanMealResponse?>.Failure(MealPlanErrors.UnableToCreate);

        return Result<PlanMealResponse?>.Success(ToDto(createdPlan));
    }

    public async Task<Result<PlanMealResponse?>> GetMealPlanByIdAsync(Guid userId, Guid planMealId)
    {
        var plan = await _mealPlanRepository.GetMealPlanByIdAsync(planMealId);
        if (plan == null)
            return Result<PlanMealResponse?>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasAccessToPlan(userId, plan.PlanId))
            return Result<PlanMealResponse?>.Failure(MealPlanErrors.Unauthorized);

        return Result<PlanMealResponse?>.Success(ToDto(plan));
    }

    public async Task<Result<PlanMealResponse>> UpdateMealPlanAsync(Guid userId, UpdatePlanMealRequest mealPlan)
    {
        var existingPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlan.Id);
        if (existingPlan == null)
            return Result<PlanMealResponse>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasEditAccessToPlan(userId, existingPlan.PlanId))
            return Result<PlanMealResponse>.Failure(MealPlanErrors.Unauthorized);

        existingPlan.MealId = mealPlan.MealId;
        existingPlan.ServeDate = mealPlan.ServeDate;
        existingPlan.EndDate = mealPlan.EndDate;

        var success = await _mealPlanRepository.UpdateMealPlanAsync(existingPlan);
        if (!success)
            return Result<PlanMealResponse>.Failure(MealPlanErrors.UnableToUpdate);

        return Result<PlanMealResponse>.Success(ToDto(existingPlan));
    }

    public async Task<Result<bool>> DeleteMealPlanAsync(Guid userId, Guid id)
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

    public async Task<Result<IEnumerable<PlanMealResponse>>> GetMealPlansForUserAsync(Guid userId)
    {
        var plans = await _mealPlanRepository.GetMealPlansForUserAsync(userId);
        return Result<IEnumerable<PlanMealResponse>>.Success(plans.Select(ToDto).ToList());
    }

    public async Task<Result<IEnumerable<PlanMealResponse>>> GetMealPlansByStartDateAsync(Guid userId, DateTime startDate)
    {
        var plans = await _mealPlanRepository.GetMealPlansByStartDateAsync(startDate);
        return Result<IEnumerable<PlanMealResponse>>.Success(await FilterToUserMealPlans(userId, plans));
    }

    public async Task<Result<IEnumerable<PlanMealResponse>>> GetMealPlansByEndDateAsync(Guid userId, DateTime endDate)
    {
        var plans = await _mealPlanRepository.GetMealPlansByEndDateAsync(endDate);
        return Result<IEnumerable<PlanMealResponse>>.Success(await FilterToUserMealPlans(userId, plans));
    }

    public async Task<Result<IEnumerable<PlanMealResponse>>> GetMealPlansByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        var plans = await _mealPlanRepository.GetMealPlansByDateRangeAsync(startDate, endDate);
        return Result<IEnumerable<PlanMealResponse>>.Success(await FilterToUserMealPlans(userId, plans));
    }

    #endregion

    #region MealItemPlan management

    public async Task<Result<PlanMealItemResponse?>> AddMealItemToPlanAsync(Guid userId, Guid mealPlanId, CreatePlanMealItemRequest mealItemPlan)
    {
        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlanId);
        if (mealPlan == null)
            return Result<PlanMealItemResponse?>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasEditAccessToPlan(userId, mealPlan.PlanId))
            return Result<PlanMealItemResponse?>.Failure(MealPlanErrors.Unauthorized);

        var mealItemPlanEntity = new PlanMealItem
        {
            MealPlanId = mealPlanId,
            MealItemId = mealItemPlan.MealItemId,
            AssignedToGuestName = mealItemPlan.AssignedToGuestName,
            AssignedToUserId = mealItemPlan.AssignedToUser,
            
            Notes = mealItemPlan.Notes?.ToString() ?? string.Empty
        };
        if (mealItemPlan.StatusTypeId.HasValue)
        {
            mealItemPlanEntity.StatusId = mealItemPlan.StatusTypeId.Value;
        }
        else
        {
            var itemStatus = await GetMealItemPlanStatusType(mealItemPlan.StatusTypeName);
            if (!itemStatus.IsSuccess)
                return Result<PlanMealItemResponse?>.Failure(MealItemStatusErrors.MealItemStatusNotFound);
            mealItemPlanEntity.StatusId = itemStatus.Value!.Id;
        }
        
        var created = await _mealItemPlanRepository.AddMealItemToMealPlanAsync(mealItemPlanEntity);
        if (created == null)
            return Result<PlanMealItemResponse?>.Failure(MealPlanErrors.UnableToCreate);

        return Result<PlanMealItemResponse?>.Success(ToItemDto(created));
    }

    public async Task<Result<IEnumerable<PlanMealItemResponse>>> GetMealItemsForMealPlanAsync(Guid userId, Guid mealPlanId)
    {
        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlanId);
        if (mealPlan == null)
            return Result<IEnumerable<PlanMealItemResponse>>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasAccessToPlan(userId, mealPlan.PlanId))
            return Result<IEnumerable<PlanMealItemResponse>>.Failure(MealPlanErrors.Unauthorized);

        var items = await _mealItemPlanRepository.GetMealItemsForMealPlanAsync(mealPlanId);
        return Result<IEnumerable<PlanMealItemResponse>>.Success(items.Select(ToItemDto).ToList());
    }

    public async Task<Result<PlanMealItemResponse>> UpdateMealItemInPlanAsync(Guid userId, Guid mealPlanId, Guid mealItemId, UpdatePlanMealItemRequest mealItemPlan)
    {
        var mealPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlanId);
        if (mealPlan == null)
            return Result<PlanMealItemResponse>.Failure(MealPlanErrors.MealPlanNotFound);
        if (!await UserHasEditAccessToPlan(userId, mealPlan.PlanId))
            return Result<PlanMealItemResponse>.Failure(MealPlanErrors.Unauthorized);

        var items = await _mealItemPlanRepository.GetMealItemsForMealPlanAsync(mealItemPlan.MealPlanId);
        var entity = items.FirstOrDefault(mip => mip.Id == mealItemPlan.Id);
        if (entity == null)
            return Result<PlanMealItemResponse>.Failure(MealPlanErrors.MealPlanNotFound);


        entity.MealItemId = mealItemPlan.MealItemId ?? entity.MealItemId;
        entity.AssignedToGuestName = mealItemPlan.AssignedToGuestName ?? entity.AssignedToGuestName;
        entity.AssignedToUserId = mealItemPlan.AssignedToUser ?? entity.AssignedToUserId;
        entity.Notes = mealItemPlan.Notes ?? entity.Notes;
        if (mealItemPlan.StatusTypeId.HasValue)
        {
            entity.StatusId = mealItemPlan.StatusTypeId.Value;
        }
        else
        {
            var itemStatus = await GetMealItemPlanStatusType(mealItemPlan.StatusTypeName);
            if (!itemStatus.IsSuccess)
                return Result<PlanMealItemResponse>.Failure(MealItemStatusErrors.MealItemStatusNotFound);
            entity.StatusId = itemStatus.Value!.Id;
        }
        var success = await _mealItemPlanRepository.UpdateMealItemInMealPlanAsync(entity);
        if (!success)
            return Result<PlanMealItemResponse>.Failure(MealPlanErrors.UnableToUpdate);

        return Result<PlanMealItemResponse>.Success(ToItemDto(entity));
    }

    public async Task<Result<bool>> RemoveMealItemFromPlanAsync(Guid userId, Guid mealItemPlanId)
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

    private async Task<IEnumerable<PlanMealResponse>> FilterToUserMealPlans(Guid userId, IEnumerable<PlanMeal> plans)
    {
        var result = new List<PlanMealResponse>();
        var checkedPlanIds = new Dictionary<Guid, bool>();
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

    private static PlanMealResponse ToDto(PlanMeal plan) => new(
        Id: plan.Id,
        MealId: plan.MealId,
        PlanId: plan.PlanId,
        ServeDate: plan.ServeDate,
        EndDate: plan.EndDate,
        AddedByUserId: plan.AddedByUserId,
        CreatedAt: plan.CreatedAt,
        UpdatedAt: plan.UpdatedAt
    );

    private static PlanMealItemResponse ToItemDto(PlanMealItem mip) => new(
        Id: mip.Id,
        MealPlanId: mip.MealPlanId,
        MealItemId: mip.MealItemId,
        AssignedToGuestName: mip.AssignedToGuestName,
        AssignedToUser: mip.AssignedToUserId,
        StatusTypeId: mip.StatusId,
        StatusTypeName: mip.MealItemPlanStatusType.Name,
        Notes: mip.Notes,
        CreatedAt: mip.CreatedAt,
        UpdatedAt: mip.UpdatedAt
    );

    private async Task<Result<MealItemPlanStatusType>> GetMealItemPlanStatusType(string name)
    {
        var mealItemStatusType = await _planMealItemStatusTypeRepository.GetByNameAsync(name);
        if (mealItemStatusType == null)
            return Result<MealItemPlanStatusType>.Failure(MealItemStatusErrors.MealItemStatusNotFound);
        return Result<MealItemPlanStatusType>.Success(mealItemStatusType);
    }
    #endregion
}
