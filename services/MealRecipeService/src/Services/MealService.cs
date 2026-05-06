using MealRecipeService.Mappings;
using MealRecipeService.Models;
using MealRecipeService.Interfaces;
using MealRecipeService.Clients;
using Shared.Models;

namespace MealRecipeService.Services;

public class MealService : IMealService
{
    private readonly IMealRepository _mealRepository;
    private readonly IMealItemRepository _mealItemRepository;
    private readonly IIdentityServiceClient _identityClient;

    public MealService(IMealRepository mealRepository, IMealItemRepository mealItemRepository, IIdentityServiceClient identityClient)
    {
        _mealRepository = mealRepository;
        _mealItemRepository = mealItemRepository;
        _identityClient = identityClient;
    }

    #region Meal operations

    public async Task<Result<MealDto>> CreateMealAsync(int userId, MealCreateDto mealCreateDto)
    {
        var meal = new Meal
        {
            Name = mealCreateDto.Name,
            Description = mealCreateDto.Description,
            Notes = mealCreateDto.Notes,
            MealType = EnumMappings.ToEntityMealType(mealCreateDto.MealType),
            IsMultiDayMeal = mealCreateDto.IsMultiDayMeal,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OwenedByUserId = userId
        };
        var newMeal = await _mealRepository.CreateAsync(meal);
        if (newMeal == null)
            return Result<MealDto>.Failure(MealErrors.UnableToCreate);

        var newMealDto = new MealDto(
            Id: newMeal.Id,
            Name: newMeal.Name,
            Description: newMeal.Description,
            Notes: newMeal.Notes,
            MealType: newMeal.MealType.ToDtoMealType(),
            IsMultiDayMeal: newMeal.IsMultiDayMeal ?? false,
            CreateAt: newMeal.CreatedAt,
            UpdatedAt: newMeal.UpdatedAt
        );
        return Result<MealDto>.Success(newMealDto);
    }

    public async Task<Result<bool>> DeleteMealAsync(int userId, int id)
    {
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
            return Result<bool>.Failure(MealErrors.NotFound);
        if (meal.OwenedByUserId != userId)
            return Result<bool>.Failure(MealErrors.Unauthorized);
        var deleted = await _mealRepository.DeleteAsync(id);
        if (!deleted)
            return Result<bool>.Failure(MealErrors.UnableToDelete);
        return Result<bool>.Success(true);
    }

    public async Task<Result<MealDto>> GetMealByIdAsync(int id)
    {
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
            return Result<MealDto>.Failure(MealErrors.NotFound);
        var mealDto = new MealDto(
            Id: meal.Id,
            Name: meal.Name,
            Description: meal.Description,
            Notes: meal.Notes,
            MealType: meal.MealType.ToDtoMealType(),
            IsMultiDayMeal: meal.IsMultiDayMeal ?? false,
            CreateAt: meal.CreatedAt,
            UpdatedAt: meal.UpdatedAt
        );
        return Result<MealDto>.Success(mealDto);
    }

    public async Task<Result<MealDto>> UpdateMealAsync(int userId, MealUpdateDto mealDto)
    {
        var mealEntity = new Meal
        {
            Id = mealDto.Id,
            Name = mealDto.Name ?? string.Empty,
            Description = mealDto.Description,
            Notes = mealDto.Notes,
            MealType = EnumMappings.ToEntityMealType(mealDto.MealType),
            IsMultiDayMeal = mealDto.IsMultiDayMeal,
            UpdatedAt = DateTime.UtcNow
        };
        var updatedMeal = await _mealRepository.UpdateAsync(mealEntity);
        if (!updatedMeal)
            return Result<MealDto>.Failure(MealErrors.UnableToUpdate);
        var updatedMealDto = new MealDto(
            Id: mealEntity.Id,
            Name: mealEntity.Name,
            Description: mealEntity.Description,
            Notes: mealEntity.Notes,
            MealType: mealEntity.MealType.ToDtoMealType(),
            IsMultiDayMeal: mealEntity.IsMultiDayMeal ?? false,
            CreateAt: null,
            UpdatedAt: mealEntity.UpdatedAt
        );
        return Result<MealDto>.Success(updatedMealDto);
    }

    #endregion

    #region MealItem operations

    public async Task<Result<MealItemDto>> AddMealItemAsync(MealItemCreateDto mealItemDto)
    {
        var mealItemEntity = new MealItem
        {
            Name = mealItemDto.Name,
            MealId = mealItemDto.MealId,
            RecipeId = mealItemDto.RecipeId,
            ItemType = EnumMappings.ToEntityItemType(mealItemDto.ItemType ?? Shared.Models.ItemType.Recipe)
        };
        var newMealItem = await _mealItemRepository.CreateAsync(mealItemEntity);
        if (newMealItem == null)
            return Result<MealItemDto>.Failure(MealErrors.UnableToCreate);
        var newMealItemDto = new MealItemDto(
            Id: newMealItem.Id,
            Name: newMealItem.Name,
            MealId: newMealItem.MealId,
            RecipeId: newMealItem.RecipeId,
            ItemType: newMealItem.ItemType.ToDtoItemType()
        );
        return Result<MealItemDto>.Success(newMealItemDto);
    }

    public async Task<Result<IEnumerable<MealItemDto>>> GetMealItemByMealIdAsync(int mealId)
    {
        var mealItemsResult = await _mealItemRepository.GetByMealIdAsync(mealId);
        if (!mealItemsResult.Any())
            return Result<IEnumerable<MealItemDto>>.Failure(MealItemErrors.NotFoundMeal);
        var mealItemDtos = mealItemsResult
            .Where(mi => mi.ItemType == Models.ItemType.Recipe && mi.RecipeId.HasValue)
            .Select(mi => new MealItemDto(
                mi.Id,
                mi.Name,
                mi.MealId,
                mi.RecipeId.HasValue ? mi.RecipeId.Value : 0,
                mi.ItemType.ToDtoItemType()
            ));
        return Result<IEnumerable<MealItemDto>>.Success(mealItemDtos);
    }

    public async Task<Result<bool>> DeleteMealItemAsync(int mealItemId)
    {
        var mealItem = await _mealItemRepository.GetByIdAsync(mealItemId);
        if (mealItem == null)
            return Result<bool>.Failure(MealErrors.NotFound);
        var deleted = await _mealItemRepository.DeleteAsync(mealItemId);
        if (!deleted)
            return Result<bool>.Failure(MealErrors.UnableToDelete);
        return Result<bool>.Success(true);
    }

    public async Task<Result<MealItemDto>> UpdateMealItemAsync(MealItemUpdateDto mealItemDto)
    {
        var mealItemEntity = new MealItem
        {
            Id = mealItemDto.Id,
            Name = mealItemDto.Name,
            MealId = mealItemDto.MealId,
            RecipeId = mealItemDto.RecipeId,
            ItemType = EnumMappings.ToEntityItemType(mealItemDto.ItemType ?? Shared.Models.ItemType.Recipe)
        };
        var updatedMealItem = await _mealItemRepository.UpdateAsync(mealItemEntity);
        if (!updatedMealItem)
            return Result<MealItemDto>.Failure(MealErrors.UnableToUpdate);
        var updatedMealItemDto = new MealItemDto(
            Id: mealItemEntity.Id,
            Name: mealItemEntity.Name,
            MealId: mealItemEntity.MealId,
            RecipeId: mealItemEntity.RecipeId,
            ItemType: mealItemEntity.ItemType.ToDtoItemType()
        );
        return Result<MealItemDto>.Success(updatedMealItemDto);
    }

    #endregion

    #region Meal share operations

    public async Task<Result<ResourcePermissionDto>> ShareMealAsync(int userId, int mealId, ShareRequestDto request)
    {
        var meal = await _mealRepository.GetByIdAsync(mealId);
        if (meal == null)
            return Result<ResourcePermissionDto>.Failure(MealErrors.NotFound);
        if (meal.OwenedByUserId != userId)
            return Result<ResourcePermissionDto>.Failure(MealErrors.Unauthorized);

        var createDto = new ResourcePermissionCreateDto(
            ResourceType: ResourceType.Meal,
            ResourceId: mealId,
            SubjectType: request.SubjectType,
            SubjectId: request.SubjectId,
            Permission: request.Permission,
            GrantedBy: userId,
            ExpiresAt: request.ExpiresAt
        );
        return await _identityClient.GrantPermissionAsync(createDto);
    }

    #endregion
}
