
using MealRecipeService.Models;
using MealRecipeService.Interfaces;
 
using Shared.Models;
using MealRecipeService.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace MealRecipeService.Services;

public class MealService : IMealService
{
    private readonly IMealRepository _mealRepository;
    private readonly IMealItemRepository _mealItemRepository;
    private readonly IAccessService _accessService;
    private readonly ILogger<MealService> _logger;
    private readonly IMealTypeRepository _mealTypeRepository;
    private readonly IMealItemTypeRepository _mealItemTypeRepository;
    private readonly ICachedUserRepository _cachedUserRepository;
    private readonly ICachedGroupRepository _cachedGroupRepository;

    public MealService(IMealRepository mealRepository, IMealItemRepository mealItemRepository, IMealItemTypeRepository mealItemTypeRepository, IMealTypeRepository mealTypeRepository, ICachedUserRepository cachedUserRepository, ICachedGroupRepository cachedGroupRepository, ILogger<MealService> logger, IAccessService accessService)
    {
        _mealRepository = mealRepository;
        _mealItemRepository = mealItemRepository;
        _mealItemTypeRepository = mealItemTypeRepository;
        _mealTypeRepository = mealTypeRepository;
        _cachedUserRepository = cachedUserRepository;
        _cachedGroupRepository = cachedGroupRepository;
        _logger = logger;
        _accessService = accessService;

    }

    #region Meal operations

    public async Task<Result<MealDetailResponse>> CreateMealAsync(Guid userId, CreateMealRequest mealCreateDto)
    {
        var meal = new Meal
        {
            Name = mealCreateDto.Name,
            Description = mealCreateDto.Description,
            Notes = mealCreateDto.Notes,
            MealTypeId = mealCreateDto.MealTypeId,
            IsMultiDayMeal = mealCreateDto.IsMultiDayMeal,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OwnerUserId = userId
        };
        var newMeal = await _mealRepository.CreateAsync(meal);
        if (newMeal == null)
            return Result<MealDetailResponse>.Failure(MealErrors.UnableToCreate);

        return Result<MealDetailResponse>.Success(ToMealDetailDto(newMeal));
    }
    public async Task<Result<IEnumerable<MealSummaryResponse>>> GetAllMealsAsync(Guid userId)
    {
        //List<MealSummaryResponse> mealSummaryResponses = new List<MealSummaryResponse>();
        var meals = await _mealRepository.GetByOwnerIdAsync(userId);



        //get meals shared with user

        var sharedPermissions = await _accessService.GetMealsSharedWithUser(userId);

        var sharedMealIds = sharedPermissions.IsSuccess
        ? sharedPermissions.Value!.Select(p => p.ResourceId).ToHashSet()
        : new HashSet<Guid>();

        var shareMeals = sharedMealIds.Any() ? await _mealRepository.GetByIdsAsync(sharedMealIds) : Enumerable.Empty<Meal>();

        meals.Concat(shareMeals);

        //create meal dto
        var mealSummaryResponses = meals.Select(meal => new MealSummaryResponse(
            Id: meal.Id,
            Name: meal.Name,
            Description: meal.Description,
            Notes: meal.Notes,
            MealTypeId: meal.MealTypeId,
            IsMultiDayMeal: meal.IsMultiDayMeal,
            OwnerUserId: meal.OwnerUserId
        ));
        // foreach (var meal in meals)
        // {
        //     var mealType = await _mealTypeRepository.GetByIdAsync(meal.MealTypeId);

        //     var mt = new MealTypeResponse(mealType.Id, mealType.Name, mealType.DisplayName, mealType.SortOrder);

        //     //create meal dto
        //     var mealDto = new MealSummaryResponse(
        //         Id: meal.Id,
        //         Name: meal.Name,
        //         Description: meal.Description,
        //         Notes: meal.Notes,
        //         MealType: mt,
        //         IsMultiDayMeal: meal.IsMultiDayMeal,
        //         OwnerUserId: meal.OwnerUserId
        //     );

        //     mealSummaryResponses.Add(mealDto);
        // }




        return Result<IEnumerable<MealSummaryResponse>>.Success(mealSummaryResponses);
    }
    public async Task<Result<IEnumerable<MealSummaryResponse>>> GetMealsSharedWithMeAsync(Guid userId)
    {
        //List<MealSummaryResponse> mealSummaryResponses = new List<MealSummaryResponse>();
        //get meals shared with user
        var sharedPermissions = await _accessService.GetMealsSharedWithUser(userId);

        var sharedMealIds = sharedPermissions.IsSuccess
        ? sharedPermissions.Value!.Select(p => p.ResourceId).ToHashSet()
        : new HashSet<Guid>();

        var shareMeals = sharedMealIds.Any() ? await _mealRepository.GetByIdsAsync(sharedMealIds) : Enumerable.Empty<Meal>();

        var mealSummaryResponses = shareMeals.Select(meal => new MealSummaryResponse(
                    Id: meal.Id,
                    Name: meal.Name,
                    Description: meal.Description,
                    Notes: meal.Notes,
                    MealTypeId: meal.MealTypeId,
                    IsMultiDayMeal: meal.IsMultiDayMeal,
                    OwnerUserId: meal.OwnerUserId
                ));
        // foreach (var meal in shareMeals)
        // {
        //     var mealType = await _mealTypeRepository.GetByIdAsync(meal.MealTypeId);

        //     var mt = new MealTypeResponse(mealType.Id, mealType.Name, mealType.DisplayName, mealType.SortOrder);

        //     //create meal dto
        //     var mealDto = new MealSummaryResponse(
        //         Id: meal.Id,
        //         Name: meal.Name,
        //         Description: meal.Description,
        //         Notes: meal.Notes,
        //         MealType: mt,
        //         IsMultiDayMeal: meal.IsMultiDayMeal,
        //         OwnerUserId: meal.OwnerUserId
        //     );

        //     mealSummaryResponses.Add(mealDto);
        // }

        return Result<IEnumerable<MealSummaryResponse>>.Success(mealSummaryResponses);
    }
    public async Task<Result<MealDetailResponse>> GetMealByIdAsync(Guid userId, Guid mealId)
    {
        var meal = await _mealRepository.GetByIdAsync(mealId);
        if (meal == null)
            return Result<MealDetailResponse>.Failure(MealErrors.NotFound);
        if (!await UserHasAccessToMeal(userId, mealId))
            return Result<MealDetailResponse>.Failure(MealErrors.Unauthorized);

        // var mt = await _mealTypeRepository.GetByIdAsync(meal.MealTypeId);
        // var mtResponse = new MealTypeResponse(mt.Id, mt.Name, mt.DisplayName, mt.SortOrder);
        var mealResponse = new MealDetailResponse(
            Id: meal.Id,
            Name: meal.Name,
            Description: meal.Description,
            Notes: meal.Notes,
            MealTypeId: meal.MealTypeId,
            IsMultiDayMeal: meal.IsMultiDayMeal,
            OwnerUserId: meal.OwnerUserId,
            CreatedAt: meal.CreatedAt,
            UpdatedAt: meal.UpdatedAt,
            UpdatedBy: meal.UpdatedBy
        );

        return Result<MealDetailResponse>.Success(mealResponse);
    }

    public async Task<Result<MealDetailResponse>> UpdateMealAsync(Guid userId, UpdateMealRequest mealDto)
    {
        if (!await UserHasEditAccessToMeal(userId, mealDto.Id))
            return Result<MealDetailResponse>.Failure(MealErrors.Unauthorized);

        var mealEntity = new Meal
        {
            Id = mealDto.Id,
            Name = mealDto.Name ?? string.Empty,
            Description = mealDto.Description,
            Notes = mealDto.Notes,
            MealTypeId = mealDto.MealTypeId,
            IsMultiDayMeal = mealDto.IsMultiDayMeal,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow
        };
        var updatedMeal = await _mealRepository.UpdateAsync(mealEntity);
        if (!updatedMeal)
            return Result<MealDetailResponse>.Failure(MealErrors.UnableToUpdate);

        return Result<MealDetailResponse>.Success(ToMealDetailDto(mealEntity));
    }

    public async Task<Result<bool>> DeleteMealAsync(Guid userId, Guid mealId)
    {
        var meal = await _mealRepository.GetByIdAsync(mealId);
        if (meal == null)
            return Result<bool>.Failure(MealErrors.NotFound);
        if (!await UserHasEditAccessToMeal(userId, mealId))
            return Result<bool>.Failure(MealErrors.Unauthorized);

        var deleted = await _mealRepository.DeleteAsync(mealId);
        if (!deleted)
            return Result<bool>.Failure(MealErrors.UnableToDelete);
        return Result<bool>.Success(true);
    }

    public async Task<Result<MealDetailResponse>> CloneMealAsync(Guid userId, Guid mealId)
    {

        var source = await _mealRepository.GetByIdAsync(mealId);
        if (source == null)
            return Result<MealDetailResponse>.Failure(MealErrors.NotFound);

        if (!await UserHasAccessToMeal(userId, mealId))
            return Result<MealDetailResponse>.Failure(MealErrors.Unauthorized);

           
        var clone = new Meal
        {
            Name = source.Name + " (Copy)",
            Description = source.Description,
            Notes = source.Notes,
            MealType = source.MealType,
            IsMultiDayMeal = source.IsMultiDayMeal,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OwnerUserId = userId
        };
        var newMeal = await _mealRepository.CreateAsync(clone);
        if (newMeal == null)
            return Result<MealDetailResponse>.Failure(MealErrors.UnableToCreate);

        var items = await _mealItemRepository.GetByMealIdAsync(mealId);
        foreach (var item in items)
        {
            await _mealItemRepository.CreateAsync(new MealItem
            {
                Name = item.Name,
                MealId = newMeal.Id,
                RecipeId = item.RecipeId,
                ItemType = item.ItemType
            });
        }

        return Result<MealDetailResponse>.Success(ToMealDetailDto(newMeal));
    }

    #endregion

    #region MealItem operations

    public async Task<Result<MealItemDetailResponse>> AddMealItemAsync(Guid userId, CreateMealItemRequest mealItemRequest)
    {
        if (!await UserHasEditAccessToMeal(userId, mealItemRequest.MealId))
            return Result<MealItemDetailResponse>.Failure(MealErrors.Unauthorized);

        var mealItemEntity = new MealItem
        {
            Name = mealItemRequest.Name,
            MealId = mealItemRequest.MealId,
            RecipeId = mealItemRequest.RecipeId,
            ItemTypeId = mealItemRequest.ItemTypeId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = mealItemRequest.CreatedBy
        };
        var newMealItem = await _mealItemRepository.CreateAsync(mealItemEntity);
        if (newMealItem == null)
            return Result<MealItemDetailResponse>.Failure(MealErrors.UnableToCreate);

        return Result<MealItemDetailResponse>.Success(ToMealItemDto(newMealItem));
    }

    public async Task<Result<IEnumerable<MealItemDetailResponse>>> GetMealItemsByMealIdAsync(Guid userId, Guid mealId)
    {
        if (!await UserHasAccessToMeal(userId, mealId))
            return Result<IEnumerable<MealItemDetailResponse>>.Failure(MealErrors.Unauthorized);

        var mealItemsResult = await _mealItemRepository.GetByMealIdAsync(mealId);
        if (!mealItemsResult.Any())
            return Result<IEnumerable<MealItemDetailResponse>>.Failure(MealItemErrors.NotFoundMeal);


        return Result<IEnumerable<MealItemDetailResponse>>.Success(mealItemsResult.Select(mi => ToMealItemDto(mi)));
    }

    public async Task<Result<bool>> DeleteMealItemAsync(Guid userId, Guid mealItemId)
    {
        var mealItem = await _mealItemRepository.GetByIdAsync(mealItemId);
        if (mealItem == null)
            return Result<bool>.Failure(MealErrors.NotFound);
        if (!await UserHasEditAccessToMeal(userId, mealItem.MealId))
            return Result<bool>.Failure(MealErrors.Unauthorized);

        var deleted = await _mealItemRepository.DeleteAsync(mealItemId);
        if (!deleted)
            return Result<bool>.Failure(MealErrors.UnableToDelete);
        return Result<bool>.Success(true);
    }

    public async Task<Result<MealItemDetailResponse>> UpdateMealItemAsync(Guid userId, UpdateMealItemRequest mealItemRequest)
    {
        var existing = await _mealItemRepository.GetByIdAsync(mealItemRequest.Id);
        if (existing == null)
            return Result<MealItemDetailResponse>.Failure(MealErrors.NotFound);
        if (!await UserHasEditAccessToMeal(userId, existing.MealId))
            return Result<MealItemDetailResponse>.Failure(MealErrors.Unauthorized);

        var mealItemEntity = new MealItem
        {
            Id = mealItemRequest.Id,
            Name = mealItemRequest.Name,
            RecipeId = mealItemRequest.RecipeId,
            ItemTypeId = mealItemRequest.ItemTypeId,
            UpdatedBy = mealItemRequest.UpdatedBy,
            UpdatedAt = DateTime.UtcNow
        };
        var updatedMealItem = await _mealItemRepository.UpdateAsync(mealItemEntity);
        if (!updatedMealItem)
            return Result<MealItemDetailResponse>.Failure(MealErrors.UnableToUpdate);

        return Result<MealItemDetailResponse>.Success(ToMealItemDto(mealItemEntity));
    }

    #endregion

    #region Meal share operations

    public async Task<Result<ShareMealResponse>> ShareMealAsync(ShareMealRequest shareMealRequest)
    {
        var meal = await _mealRepository.GetByIdAsync(shareMealRequest.MealId);
        if (meal == null)
            return Result<ShareMealResponse>.Failure(MealErrors.NotFound);
        if (meal.OwnerUserId != shareMealRequest.GrantedBy)
            return Result<ShareMealResponse>.Failure(MealErrors.Unauthorized);

        var permissionRequest = new CreateResourcePermissionRequest(
            "meal",
            shareMealRequest.MealId,
            shareMealRequest.SubjectTypeName,
            shareMealRequest.SubjectId,
            shareMealRequest.SubjectTypeName == "group" ? "view" : shareMealRequest.PermissionTypeName,
            shareMealRequest.GrantedBy,
            shareMealRequest.ExpiresAt
            );

        var accessResponse = await _accessService.GrantAccessToResource(permissionRequest);
        if (accessResponse.IsSuccess == false)
            return Result<ShareMealResponse>.Failure(MealErrors.UnableToShare);

        var response = new ShareMealResponse(
            accessResponse.Value!.ResourceId,
            accessResponse.Value!.ResourceTypeName,
            accessResponse.Value!.ResourceTypeId,
            accessResponse.Value!.SubjectTypeName,
            accessResponse.Value!.SubjectTypeId,
            accessResponse.Value!.PermissionTypeName,
            accessResponse.Value!.PermissionTypeId,
            accessResponse.Value!.SubjectId,
            accessResponse.Value!.GrantedBy,
            accessResponse.Value!.ExpiresAt);

        return Result<ShareMealResponse>.Success(response);

    }

    #endregion

    #region private helper methods

    private async Task<bool> UserHasAccessToMeal(Guid userId, Guid mealId)
    {
        var meal = await _mealRepository.GetByIdAsync(mealId);
        if (meal == null) return false;
        if (meal.OwnerUserId == userId) return true;

        var mealsShared = await _accessService.GetMealsSharedWithUser(userId);

        return mealsShared.IsSuccess && mealsShared.Value!.Any(p => p.ResourceId == mealId);
    }

    private async Task<bool> UserHasEditAccessToMeal(Guid userId, Guid mealId)
    {
        var meal = await _mealRepository.GetByIdAsync(mealId);
        if (meal == null) return false;
        if (meal.OwnerUserId == userId) return true;

        var permissions = await _accessService.GetMealsSharedWithUser(userId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.ResourceId == mealId && p.PermissionTypeName == "edit");
    }

    private static MealDetailResponse ToMealDetailDto(Meal meal) => new(
        Id: meal.Id,
        Name: meal.Name,
        Description: meal.Description,
        Notes: meal.Notes,
        MealTypeId: meal.MealTypeId,
        IsMultiDayMeal: meal.IsMultiDayMeal,
        OwnerUserId: meal.OwnerUserId,
        CreatedAt: meal.CreatedAt,
        UpdatedBy: meal.UpdatedBy,
        UpdatedAt: meal.UpdatedAt
    );

    private static MealItemDetailResponse ToMealItemDto(MealItem item) => new(
        Id: item.Id,
        Name: item.Name,
        MealId: item.MealId,
        RecipeId: item.RecipeId,
        ItemTypeId: item.ItemTypeId,
        ItemTypeName: item.ItemType.Name,
        CreatedBy: item.CreatedBy,
        CreatedAt: item.CreatedAt,
        UpdatedBy: item.UpdatedBy,
        UpdatedAt: item.UpdatedAt

    );

    #endregion
}
