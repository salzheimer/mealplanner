using PlanService.Interfaces;
using PlanService.Mappings;
using PlanService.Models;
using Shared.Models;
namespace PlanService.Services;

public class MealPlanService : IMealPlanService
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IMealItemPlanRepository _mealItemPlanRepository;
    public MealPlanService(IMealPlanRepository mealPlanRepository, IMealItemPlanRepository mealItemPlanRepository)
    {
        _mealPlanRepository = mealPlanRepository;
        _mealItemPlanRepository = mealItemPlanRepository;
    }

    #region MealPlan management

    public async Task<Result<MealPlanDto?>> CreateMealPlanAsync(MealPlanCreateDto mealPlan)
    {
         var plan= new MealPlan{
            MealId = mealPlan.MealId,
            PlanId = mealPlan.PlanId,
            ServeDate = mealPlan.ServeDate,
            EndDate = mealPlan.EndDate,
            AddedByUserId = mealPlan.AddedByUserId
         };
         var createdPlan =await _mealPlanRepository.CreateMealPlanAsync(plan);
    
            if (createdPlan == null)
            {
                return Result<MealPlanDto?>.Failure(MealPlanErrors.UnableToCreate);
            }   

            var mealPlanDto = new MealPlanDto(
                Id: createdPlan.Id,               
                MealId: createdPlan.MealId,
                PlanId: createdPlan.PlanId,
                ServeDate: createdPlan.ServeDate,
                EndDate: createdPlan.EndDate,
                AddedByUserId: createdPlan.AddedByUserId,
                CreatedAt: createdPlan.CreatedAt,
                UpdatedAt: createdPlan.UpdatedAt
            );

            return Result<MealPlanDto?>.Success(mealPlanDto);
    }

    public async Task<Result<bool>> DeleteMealPlanAsync(int id)
    {
        var success = await _mealPlanRepository.DeleteMealPlanAsync(id);
        if (!success)
        {
            return Result<bool>.Failure(MealPlanErrors.UnableToDelete);
        }
        return Result<bool>.Success(true);
    }



    public async Task<Result<MealPlanDto?>> GetMealPlanByIdAsync(int id)
    {
        var plan = await _mealPlanRepository.GetMealPlanByIdAsync(id);
        if (plan == null)
        {
            return Result<MealPlanDto?>.Failure(MealPlanErrors.MealPlanNotFound);
        }

        var mealPlanDto = new MealPlanDto(
            Id: plan.Id,
            MealId: plan.MealId,
            PlanId: plan.PlanId,
            ServeDate: plan.ServeDate,
            EndDate: plan.EndDate,
            AddedByUserId: plan.AddedByUserId,
            CreatedAt: plan.CreatedAt,
            UpdatedAt: plan.UpdatedAt
        );

        return Result<MealPlanDto?>.Success(mealPlanDto);
    }

    public async Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
       var plans = await _mealPlanRepository.GetMealPlansByDateRangeAsync(startDate, endDate);
        var mealPlanDtos = plans.Select(plan => new MealPlanDto(
            Id: plan.Id,
            MealId: plan.MealId,
            PlanId: plan.PlanId,
            ServeDate: plan.ServeDate,
            EndDate: plan.EndDate,
            AddedByUserId: plan.AddedByUserId,
            CreatedAt: plan.CreatedAt,
            UpdatedAt: plan.UpdatedAt
        )).ToList();

        return Result<IEnumerable<MealPlanDto>>.Success(mealPlanDtos);
    }

    public async Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByEndDateAsync(DateTime endDate)
    {
        var plans = await _mealPlanRepository.GetMealPlansByEndDateAsync(endDate);
        var mealPlanDtos = plans.Select(plan => new MealPlanDto(
            Id: plan.Id,
            MealId: plan.MealId,
            PlanId: plan.PlanId,
            ServeDate: plan.ServeDate,
            EndDate: plan.EndDate,
            AddedByUserId: plan.AddedByUserId,
            CreatedAt: plan.CreatedAt,
            UpdatedAt: plan.UpdatedAt
        )).ToList();

        return Result<IEnumerable<MealPlanDto>>.Success(mealPlanDtos);
    }

    public async Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansByStartDateAsync(DateTime startDate)
    {
        var plans = await _mealPlanRepository.GetMealPlansByStartDateAsync(startDate);
        var mealPlanDtos = plans.Select(plan => new MealPlanDto(
            Id: plan.Id,
            MealId: plan.MealId,
            PlanId: plan.PlanId,
            ServeDate: plan.ServeDate,
            EndDate: plan.EndDate,
            AddedByUserId: plan.AddedByUserId,
            CreatedAt: plan.CreatedAt,
            UpdatedAt: plan.UpdatedAt
        )).ToList();

        return Result<IEnumerable<MealPlanDto>>.Success(mealPlanDtos);
    }

    public async Task<Result<IEnumerable<MealPlanDto>>> GetMealPlansForUserAsync(int userId)
    {
        var plans = await _mealPlanRepository.GetMealPlansForUserAsync(userId);
        var mealPlanDtos = plans.Select(plan => new MealPlanDto(
            Id: plan.Id,
            MealId: plan.MealId,
            PlanId: plan.PlanId,
            ServeDate: plan.ServeDate,
            EndDate: plan.EndDate,
            AddedByUserId: plan.AddedByUserId,
            CreatedAt: plan.CreatedAt,
            UpdatedAt: plan.UpdatedAt
        )).ToList();

        return Result<IEnumerable<MealPlanDto>>.Success(mealPlanDtos);
    }

    #endregion


    public async Task<Result<MealPlanDto>>  UpdateMealPlanAsync(MealPlanUpdateDto mealPlan)
    {
        var existingPlan = await _mealPlanRepository.GetMealPlanByIdAsync(mealPlan.Id);
        if (existingPlan == null)
        {
            return Result<MealPlanDto>.Failure(MealPlanErrors.MealPlanNotFound);
        }

        existingPlan.MealId = mealPlan.MealId;
        existingPlan.PlanId = mealPlan.PlanId;
        existingPlan.ServeDate = mealPlan.ServeDate;
        existingPlan.EndDate = mealPlan.EndDate;
        existingPlan.AddedByUserId = mealPlan.AddedByUserId;

        var success = await _mealPlanRepository.UpdateMealPlanAsync(existingPlan);
        if (!success)
        {
            return Result<MealPlanDto>.Failure(MealPlanErrors.UnableToUpdate);
        }

        var updatedMealPlanDto = new MealPlanDto(
            Id: existingPlan.Id,
            MealId: existingPlan.MealId,
            PlanId: existingPlan.PlanId,
            ServeDate: existingPlan.ServeDate,
            EndDate: existingPlan.EndDate,
            AddedByUserId: existingPlan.AddedByUserId,
            CreatedAt: existingPlan.CreatedAt,
            UpdatedAt: existingPlan.UpdatedAt
        );

        return Result<MealPlanDto>.Success(updatedMealPlanDto);
    }
    #region  MealItemPlan management
    public async Task<Result<MealItemPlanDto?>> AddMealItemToPlanAsync(MealItemPlanCreateDto mealItemPlan)
    {
        var mealItemPlanEntity = new MealItemPlan
        {
            MealPlanId = mealItemPlan.MealPlanId,
            MealItemId = mealItemPlan.MealItemId,
            AssignedToGuestName = mealItemPlan.AssignedToGuestName,
            AssignedToUserId = mealItemPlan.AssignedToUser,
            Status = EnumMappings.ToEntityItemStatus(mealItemPlan.Status ?? Shared.Models.ItemStatus.Pending),
            Notes = mealItemPlan?.Notes?.ToString() ?? string.Empty
        };

        var createdMealItemPlan = await _mealItemPlanRepository.AddMealItemToMealPlanAsync(mealItemPlanEntity);
        if (createdMealItemPlan == null)
        {
            return Result<MealItemPlanDto?>.Failure(MealPlanErrors.UnableToCreate);
        }

        var mealItemPlanDto = new MealItemPlanDto(
            Id: createdMealItemPlan.Id,
            MealPlanId: createdMealItemPlan.MealPlanId,
            MealItemId: createdMealItemPlan.MealItemId,
            AssignedToGuestName: createdMealItemPlan.AssignedToGuestName,
            AssignedToUser: createdMealItemPlan.AssignedToUserId,
            Status: EnumMappings.ToDtoItemStatus(createdMealItemPlan.Status),
            Notes: createdMealItemPlan.Notes,
            CreatedAt: createdMealItemPlan.CreatedAt,
            UpdatedAt: createdMealItemPlan.UpdatedAt
        );

        return Result<MealItemPlanDto?>.Success(mealItemPlanDto);
    }
    public async Task<Result<IEnumerable<MealItemPlanDto>>> GetMealItemsForMealPlanAsync(int mealPlanId)
    {
        var mealItemPlans = await _mealItemPlanRepository.GetMealItemsForMealPlanAsync(mealPlanId);
        var mealItemPlanDtos = mealItemPlans.Select(mip => new MealItemPlanDto(
            Id: mip.Id,
            MealPlanId: mip.MealPlanId,
            MealItemId: mip.MealItemId,
            AssignedToGuestName: mip.AssignedToGuestName,
            AssignedToUser: mip.AssignedToUserId,
            Status: EnumMappings.ToDtoItemStatus(mip.Status),
            Notes: mip.Notes,
            CreatedAt: mip.CreatedAt,
            UpdatedAt: mip.UpdatedAt
        )).ToList();

        return Result<IEnumerable<MealItemPlanDto>>.Success(mealItemPlanDtos);
    }
    public async Task<Result<MealItemPlanDto>> UpdateMealItemInPlanAsync(MealItemPlanUpdateDto mealItemPlan)
    {
        var existingMealItemPlan = await _mealItemPlanRepository.GetMealItemsForMealPlanAsync(mealItemPlan.MealPlanId);
        var mealItemPlanEntity = existingMealItemPlan.FirstOrDefault(mip => mip.Id == mealItemPlan.Id);
        if (mealItemPlanEntity == null)
        {
            return Result<MealItemPlanDto>.Failure(MealPlanErrors.MealPlanNotFound);
        }

        mealItemPlanEntity.MealItemId = mealItemPlan.MealItemId ?? mealItemPlanEntity.MealItemId;
        mealItemPlanEntity.AssignedToGuestName = mealItemPlan.AssignedToGuestName ?? mealItemPlanEntity.AssignedToGuestName;
        mealItemPlanEntity.AssignedToUserId = mealItemPlan.AssignedToUser ?? mealItemPlanEntity.AssignedToUserId;
        mealItemPlanEntity.Status = mealItemPlan.Status != null ? EnumMappings.ToEntityItemStatus(mealItemPlan.Status.Value) : mealItemPlanEntity.Status;
        mealItemPlanEntity.Notes = mealItemPlan.Notes ?? mealItemPlanEntity.Notes;  

        var success = await _mealItemPlanRepository.UpdateMealItemInMealPlanAsync(mealItemPlanEntity);
        if (!success)        {
            return Result<MealItemPlanDto>.Failure(MealPlanErrors.UnableToUpdate);
        }   

        var updatedMealItemPlanDto = new MealItemPlanDto(
            Id: mealItemPlanEntity.Id,
            MealPlanId: mealItemPlanEntity.MealPlanId,
            MealItemId: mealItemPlanEntity.MealItemId,
            AssignedToGuestName: mealItemPlanEntity.AssignedToGuestName,
            AssignedToUser: mealItemPlanEntity.AssignedToUserId,
            Status: EnumMappings.ToDtoItemStatus(mealItemPlanEntity.Status),
            Notes: mealItemPlanEntity.Notes,
            CreatedAt: mealItemPlanEntity.CreatedAt,
            UpdatedAt: mealItemPlanEntity.UpdatedAt
        );  
        return Result<MealItemPlanDto>.Success(updatedMealItemPlanDto);
    }

    public async Task<Result<bool>> RemoveMealItemFromPlanAsync(int mealItemPlanId)
    {
        var success = await _mealItemPlanRepository.RemoveMealItemFromMealPlanAsync(mealItemPlanId);
        if (!success)
        {
            return Result<bool>.Failure(MealPlanErrors.UnableToDelete);
        }
        return Result<bool>.Success(true);
    }
    #endregion
}