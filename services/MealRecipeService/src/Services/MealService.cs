using MealRecipeService.Mappings;
using MealRecipeService.Models;
using MealRecipeService.Interfaces;
using Shared.Models;
using System.Net;

namespace MealRecipeService.Services;

public class MealService : IMealService
{
    private readonly IMealRepository _mealRepository;
    private readonly IMealItemRepository _mealItemRepository;
    private readonly IMealShareRepository _mealShareRepository;
    
    public MealService(IMealRepository mealRepository, IMealItemRepository mealItemRepository, IMealShareRepository mealShareRepository)
    {
        _mealRepository = mealRepository;
        _mealItemRepository = mealItemRepository;
        _mealShareRepository = mealShareRepository;
         
    }


    #region Meal operations

    public async Task<Result<MealDto>> CreateMealAsync(MealCreateDto mealCreateDto)
    {
        var meal = new Meal
        {
            Name = mealCreateDto.Name,
            Description = mealCreateDto.Description,
            Notes = mealCreateDto.Notes,
            MealType = EnumMappings.ToEntityMealType(mealCreateDto.MealType),
            IsMultiDayMeal = mealCreateDto.IsMultiDayMeal,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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
         Visibility: newMeal.Visibility.ToDtoVisibility(),
         CreateAt: newMeal.CreatedAt,
         UpdatedAt: newMeal.UpdatedAt
        );
        return Result<MealDto>.Success(newMealDto);
    }
    public async Task<Result<bool>> DeleteMealAsync(int id)
    {
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
        {
            return Result<bool>.Failure(MealErrors.NotFound);
        }
        var deleted = await _mealRepository.DeleteAsync(id);
        if (!deleted)
        {
            return Result<bool>.Failure(MealErrors.UnableToDelete);
        }
        return Result<bool>.Success(true);
    }
    public async Task<Result<MealDto>> GetMealByIdAsync(int id)
    {
        var meal = await _mealRepository.GetByIdAsync(id);
        if (meal == null)
        {
            return Result<MealDto>.Failure(MealErrors.NotFound);
        }
        var mealDto = new MealDto(
            Id: meal.Id,
            Name: meal.Name,
            Description: meal.Description,
            Notes: meal.Notes,
            MealType: meal.MealType.ToDtoMealType(),
            IsMultiDayMeal: meal.IsMultiDayMeal ?? false,
            Visibility: meal.Visibility.ToDtoVisibility(),
            CreateAt: meal.CreatedAt,
            UpdatedAt: meal.UpdatedAt
        );
        return Result<MealDto>.Success(mealDto);
    }

    public async Task<Result<MealDto>> UpdateMealAsync(MealUpdateDto mealDto)
    {
        var mealEntity = new Meal
        {
            Id = mealDto.Id,
            Name = mealDto.Name ?? string.Empty,
            Description = mealDto.Description,
            Notes = mealDto.Notes,
            MealType = EnumMappings.ToEntityMealType(mealDto.MealType),
            IsMultiDayMeal = mealDto.IsMultiDayMeal,
            Visibility = EnumMappings.ToEntityVisibility(mealDto.Visibility ?? Shared.Models.Visibility.Private),
            UpdatedAt = DateTime.UtcNow
        };
        var updatedMeal = await _mealRepository.UpdateAsync(mealEntity);
        if (!updatedMeal)
        {
            return Result<MealDto>.Failure(MealErrors.UnableToUpdate);
        }
        var updatedMealDto = new MealDto(
            Id: mealEntity.Id,
            Name: mealEntity.Name,
            Description: mealEntity.Description,
            Notes: mealEntity.Notes,
            MealType: mealEntity.MealType.ToDtoMealType(),
            IsMultiDayMeal: mealEntity.IsMultiDayMeal ?? false,
            Visibility: mealEntity.Visibility.ToDtoVisibility(),
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
        {
            return Result<MealItemDto>.Failure(MealErrors.UnableToCreate);
        }
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
        if (mealItemsResult.Count() == 0)
        {
            return Result<IEnumerable<MealItemDto>>.Failure(MealItemErrors.NotFoundMeal);
        }
        var mealItems = mealItemsResult;
        var mealItemDtos = mealItems
            .Where(mi => mi.ItemType == Models.ItemType.Recipe && mi.RecipeId.HasValue)
            .Select(mi => new MealItemDto
            (
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
        {
            return Result<bool>.Failure(MealErrors.NotFound);
        }
        var deleted = await _mealItemRepository.DeleteAsync(mealItemId);
        if (!deleted)
        {
            return Result<bool>.Failure(MealErrors.UnableToDelete);
        }
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
        {
            return Result<MealItemDto>.Failure(MealErrors.UnableToUpdate);
        }
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

    #region MealShare operations
    public async Task<Result<bool>> DeleteMealShareAsync(int mealShareId)
    {
        var mealShare = await _mealShareRepository.GetByIdAsync(mealShareId);
        if (mealShare == null)
        {
            return Result<bool>.Failure(MealErrors.NotFound);
        }
        var deleted = await _mealShareRepository.DeleteAsync(mealShareId);
        if (!deleted)
        {
            return Result<bool>.Failure(MealErrors.UnableToDelete);
        }
        return Result<bool>.Success(true);
    }


    public async Task<Result<MealShareDto>> ShareMealAsync(MealShareCreateDto mealShareDto)
    {
        var mealShareEntity = new MealShare
        {
            MealId = mealShareDto.MealId,
            SharedWithUserId = mealShareDto.SharedWithUserId,
            SharedByUserId = mealShareDto.SharedByUserId
        };
        var newMealShare = await _mealShareRepository.CreateAsync(mealShareEntity);
        if (newMealShare == null)
        {
            return Result<MealShareDto>.Failure(MealErrors.UnableToCreate);
        }
        var newMealShareDto = new MealShareDto(
            Id: newMealShare.Id,
            MealId: newMealShare.MealId,
            SharedWithUserId: newMealShare.SharedWithUserId,
            SharedWithGroupId: newMealShare.SharedWithGroupId,
            SharedByUserId: newMealShare.SharedByUserId,
            Permission: newMealShare.Permission.ToDtoPermission(),
            CreatedAt: newMealShare.CreatedAt,
            ExpiresAt: newMealShare.ExpiresAt
        );
        return Result<MealShareDto>.Success(newMealShareDto);
    }

    public async Task<Result<MealShareDto>> UpdateMealShareAsync(MealShareUpdateDto mealShareDto)
    {
        var mealShareEntity = new MealShare
        {
            Id = mealShareDto.Id,
            MealId = mealShareDto.MealId,
            SharedWithUserId = mealShareDto.SharedWithUserId,
            SharedWithGroupId = mealShareDto.SharedWithGroupId,
            SharedByUserId = mealShareDto.SharedByUserId,
            Permission = EnumMappings.ToEntityPermission(mealShareDto.Permission),
            ExpiresAt = mealShareDto.ExpiresAt
        };
        var updatedMealShare = await _mealShareRepository.UpdateAsync(mealShareEntity);
        if (!updatedMealShare)
        {
            return Result<MealShareDto>.Failure(MealErrors.UnableToUpdate);
        }
        var updatedMealShareDto = new MealShareDto(
            Id: mealShareEntity.Id,
            MealId: mealShareEntity.MealId,
            SharedWithUserId: mealShareEntity.SharedWithUserId,
            SharedWithGroupId: mealShareEntity.SharedWithGroupId,
            SharedByUserId: mealShareEntity.SharedByUserId,
            Permission: mealShareEntity.Permission.ToDtoPermission(),
            CreatedAt: mealShareEntity.CreatedAt,
            ExpiresAt: mealShareEntity.ExpiresAt
        );
        return Result<MealShareDto>.Success(updatedMealShareDto);
    }



    #endregion











}