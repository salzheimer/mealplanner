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
            OwnerUserId = userId
        };
        var newMeal = await _mealRepository.CreateAsync(meal);
        if (newMeal == null)
            return Result<MealDto>.Failure(MealErrors.UnableToCreate);

        return Result<MealDto>.Success(ToDto(newMeal));
    }
    public async Task<Result<IEnumerable<MealDto>>> GetAllMealsAsync(int userId)
    {
        var meals = await _mealRepository.GetByOwnerIdAsync(userId);

        //get meals shared with user
        var sharedPermissions = await _identityClient.GetUserPermissionsAsync(userId);

        var sharedMealIds = sharedPermissions.IsSuccess
        ? sharedPermissions.Value!.Where(p => p.ResourceType == ResourceType.Meal).Select(p => p.ResourceId).ToHashSet()
        : new HashSet<int>();

        var shareMeals = sharedMealIds.Any() ? await _mealRepository.GetByIdsAsync(sharedMealIds) : Enumerable.Empty<Meal>();

        meals.Concat(shareMeals);

        var mealDtos = meals.Select(m => new MealDto(
            Id: m.Id,
            Name: m.Name,
            Description: m.Description,
            Notes: m.Notes,
            MealType: Mappings.EnumMappings.ToDtoMealType(m.MealType),
            IsMultiDayMeal: m.IsMultiDayMeal,
            CreatedBy: m.CreatedBy,
            CreateAt: m.CreatedAt,
            UpdatedBy: m.UpdatedBy,
            UpdatedAt: m.UpdatedAt
        ));

        return Result<IEnumerable<MealDto>>.Success(mealDtos);
    }
    public async Task<Result<IEnumerable<MealDto>>> GetMealsSharedWithMeAsync(int userId)
    {
        
        //get meals shared with user
        var sharedPermissions = await _identityClient.GetUserPermissionsAsync(userId);

        var sharedMealIds = sharedPermissions.IsSuccess
        ? sharedPermissions.Value!.Where(p => p.ResourceType == ResourceType.Meal).Select(p => p.ResourceId).ToHashSet()
        : new HashSet<int>();

        var sharedMeals = sharedMealIds.Any() ? await _mealRepository.GetByIdsAsync(sharedMealIds) : Enumerable.Empty<Meal>();

         
        var mealDtos = sharedMeals.Select(m => new MealDto(
            Id: m.Id,
            Name: m.Name,
            Description: m.Description,
            Notes: m.Notes,
            MealType: Mappings.EnumMappings.ToDtoMealType(m.MealType),
            IsMultiDayMeal: m.IsMultiDayMeal,
            CreatedBy: m.CreatedBy,
            CreateAt: m.CreatedAt,
            UpdatedBy: m.UpdatedBy,
            UpdatedAt: m.UpdatedAt
        ));

        return Result<IEnumerable<MealDto>>.Success(mealDtos);
    }
    public async Task<Result<MealDto>> GetMealByIdAsync(int userId, int id)
    {
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
            return Result<MealDto>.Failure(MealErrors.NotFound);
        if (!await UserHasAccessToMeal(userId, id))
            return Result<MealDto>.Failure(MealErrors.Unauthorized);

        return Result<MealDto>.Success(ToDto(meal));
    }

    public async Task<Result<MealDto>> UpdateMealAsync(int userId, MealUpdateDto mealDto)
    {
        if (!await UserHasEditAccessToMeal(userId, mealDto.Id))
            return Result<MealDto>.Failure(MealErrors.Unauthorized);

        var mealEntity = new Meal
        {
            Id = mealDto.Id,
            Name = mealDto.Name ?? string.Empty,
            Description = mealDto.Description,
            Notes = mealDto.Notes,
            MealType = EnumMappings.ToEntityMealType(mealDto.MealType),
            IsMultiDayMeal = mealDto.IsMultiDayMeal,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow
        };
        var updatedMeal = await _mealRepository.UpdateAsync(mealEntity);
        if (!updatedMeal)
            return Result<MealDto>.Failure(MealErrors.UnableToUpdate);

        return Result<MealDto>.Success(new MealDto(
            Id: mealEntity.Id,
            Name: mealEntity.Name,
            Description: mealEntity.Description,
            Notes: mealEntity.Notes,
            MealType: mealEntity.MealType.ToDtoMealType(),
            IsMultiDayMeal: mealEntity.IsMultiDayMeal,
            CreatedBy: mealEntity.CreatedBy,
            CreateAt: null,
            UpdatedBy: mealEntity.UpdatedBy,
            UpdatedAt: mealEntity.UpdatedAt

        ));
    }

    public async Task<Result<bool>> DeleteMealAsync(int userId, int id)
    {
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
            return Result<bool>.Failure(MealErrors.NotFound);
        if (!await UserHasEditAccessToMeal(userId, id))
            return Result<bool>.Failure(MealErrors.Unauthorized);

        var deleted = await _mealRepository.DeleteAsync(id);
        if (!deleted)
            return Result<bool>.Failure(MealErrors.UnableToDelete);
        return Result<bool>.Success(true);
    }

    public async Task<Result<MealDto>> CloneMealAsync(int userId, int mealId)
    {
        if (!await UserHasAccessToMeal(userId, mealId))
            return Result<MealDto>.Failure(MealErrors.Unauthorized);

        var source = await _mealRepository.GetByIdAsync(mealId);
        if (source == null)
            return Result<MealDto>.Failure(MealErrors.NotFound);

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
            return Result<MealDto>.Failure(MealErrors.UnableToCreate);

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

        return Result<MealDto>.Success(ToDto(newMeal));
    }

    #endregion

    #region MealItem operations

    public async Task<Result<MealItemDto>> AddMealItemAsync(int userId, MealItemCreateDto mealItemDto)
    {
        if (!await UserHasEditAccessToMeal(userId, mealItemDto.MealId))
            return Result<MealItemDto>.Failure(MealErrors.Unauthorized);

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

        return Result<MealItemDto>.Success(ToItemDto(newMealItem));
    }

    public async Task<Result<IEnumerable<MealItemDto>>> GetMealItemByMealIdAsync(int userId, int mealId)
    {
        if (!await UserHasAccessToMeal(userId, mealId))
            return Result<IEnumerable<MealItemDto>>.Failure(MealErrors.Unauthorized);

        var mealItemsResult = await _mealItemRepository.GetByMealIdAsync(mealId);
        if (!mealItemsResult.Any())
            return Result<IEnumerable<MealItemDto>>.Failure(MealItemErrors.NotFoundMeal);

        var mealItemDtos = mealItemsResult
            .Where(mi => mi.ItemType == Models.ItemType.Recipe && mi.RecipeId.HasValue)
            .Select(mi => ToItemDto(mi));
        return Result<IEnumerable<MealItemDto>>.Success(mealItemDtos);
    }

    public async Task<Result<bool>> DeleteMealItemAsync(int userId, int mealItemId)
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

    public async Task<Result<MealItemDto>> UpdateMealItemAsync(int userId, MealItemUpdateDto mealItemDto)
    {
        var existing = await _mealItemRepository.GetByIdAsync(mealItemDto.Id);
        if (existing == null)
            return Result<MealItemDto>.Failure(MealErrors.NotFound);
        if (!await UserHasEditAccessToMeal(userId, existing.MealId))
            return Result<MealItemDto>.Failure(MealErrors.Unauthorized);

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

        return Result<MealItemDto>.Success(ToItemDto(mealItemEntity));
    }

    #endregion

    #region Meal share operations

    public async Task<Result<ResourcePermissionDto>> ShareMealAsync(int userId, int mealId, ShareRequestDto request)
    {
        var meal = await _mealRepository.GetByIdAsync(mealId);
        if (meal == null)
            return Result<ResourcePermissionDto>.Failure(MealErrors.NotFound);
        if (meal.OwnerUserId != userId)
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

    #region private helper methods

    private async Task<bool> UserHasAccessToMeal(int userId, int mealId)
    {
        var meal = await _mealRepository.GetByIdAsync(mealId);
        if (meal == null) return false;
        if (meal.OwnerUserId == userId) return true;

        var permissions = await _identityClient.GetPermissionsForResourceAsync(ResourceType.Meal, mealId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId);
    }

    private async Task<bool> UserHasEditAccessToMeal(int userId, int mealId)
    {
        var meal = await _mealRepository.GetByIdAsync(mealId);
        if (meal == null) return false;
        if (meal.OwnerUserId == userId) return true;

        var permissions = await _identityClient.GetPermissionsForResourceAsync(ResourceType.Meal, mealId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId && p.Permission >= Shared.Models.Permission.Edit);
    }

    private static MealDto ToDto(Meal meal) => new(
        Id: meal.Id,
        Name: meal.Name,
        Description: meal.Description,
        Notes: meal.Notes,
        MealType: meal.MealType.ToDtoMealType(),
        IsMultiDayMeal: meal.IsMultiDayMeal,
        CreatedBy: meal.CreatedBy,
        CreateAt: meal.CreatedAt,
        UpdatedBy: meal.UpdatedBy,
        UpdatedAt: meal.UpdatedAt
    );

    private static MealItemDto ToItemDto(MealItem item) => new(
        Id: item.Id,
        Name: item.Name,
        MealId: item.MealId,
        RecipeId: item.RecipeId,
        ItemType: item.ItemType.ToDtoItemType()
    );

    #endregion
}
