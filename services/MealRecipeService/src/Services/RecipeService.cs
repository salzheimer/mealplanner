using MealRecipeService.Mappings;
using MealRecipeService.Models;
using MealRecipeService.Interfaces;
using Shared.Models;
using System.IO.Pipelines;

namespace MealRecipeService.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeIngredientRepository _ingredientRepository;
    private readonly IRecipeInstructionRepository _instructionRepository;
    private readonly IRecipeShareRepository _shareRepository;

    public RecipeService(IRecipeRepository recipeRepository, IRecipeIngredientRepository ingredientRepository, IRecipeInstructionRepository instructionRepository, IRecipeShareRepository shareRepository)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _instructionRepository = instructionRepository;
        _shareRepository = shareRepository;
    }

    #region Recipe operations

    public async Task<Result<RecipeSummaryDto>> CreateRecipeAsync(RecipeCreateDto recipe)
    {
        var recipeEntity = new Recipe
        {
            Name = recipe.Name,
            Description = recipe.Description,
            Notes = recipe.Notes,
            Ranking = recipe.Ranking,
            OriginalSource = recipe.OriginalSource,
            CookTime = recipe.CookTime,
            PrepTime = recipe.PrepTime,
            Servings = recipe.Servings,
            OwnerUserId = recipe.OwnerUserId
        };
        var createdRecipe = await _recipeRepository.CreateAsync(recipeEntity);
        if (createdRecipe == null)
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToCreate);
        }
        var recipeDto = new RecipeSummaryDto(
            Id: createdRecipe.Id,
            Name: createdRecipe.Name,
            Description: createdRecipe.Description,
            Ranking: createdRecipe.Ranking,
            OriginalSource: createdRecipe.OriginalSource,
            CookTime: createdRecipe.CookTime,
            PrepTime: createdRecipe.PrepTime,
            Servings: createdRecipe.Servings,
            OwnerUserId: createdRecipe.OwnerUserId,
            Visibility: createdRecipe.Visibility.ToDtoVisibility()

         );
        return Result<RecipeSummaryDto>.Success(recipeDto);
    }

    public async Task<Result<bool>> DeleteRecipeAsync(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
        {
            return Result<bool>.Failure(RecipeErrors.NotFound);
        }

        var deleted = await _recipeRepository.DeleteAsync(recipe.Id);
        if (!deleted)
        {
            return Result<bool>.Failure(RecipeErrors.UnableToDelete);
        }
        return Result<bool>.Success(true);
    }
    public async Task<Result<IEnumerable<RecipeSummaryDto>>> GetAllRecipesAsync()
    {
        var recipes = await _recipeRepository.GetAllAsync();
        var recipeDtos = recipes.Select(r => new RecipeSummaryDto(
            Id: r.Id,
            Name: r.Name,
            Description: r.Description,
            Ranking: r.Ranking,
            OriginalSource: r.OriginalSource,
            CookTime: r.CookTime,
            PrepTime: r.PrepTime,
            Servings: r.Servings,
            OwnerUserId: r.OwnerUserId,
            Visibility: r.Visibility.ToDtoVisibility()
        ));
        return Result<IEnumerable<RecipeSummaryDto>>.Success(recipeDtos);
    }

    public async Task<Result<RecipeSummaryDto>> UpdateRecipeAsync(RecipeUpdateDto recipe)
    {
        var recipeEntity = new Recipe
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Notes = recipe.Notes,
            Ranking = recipe.Ranking,
            OriginalSource = recipe.OriginalSource,
            CookTime = recipe.CookTime,
            PrepTime = recipe.PrepTime,
            Servings = recipe.Servings,
            Visibility = recipe.Visibility.HasValue ? recipe.Visibility.Value.ToEntityVisibility() : Models.Visibility.Private
        };

        var updatedRecipe = await _recipeRepository.UpdateAsync(recipeEntity);
        if (!updatedRecipe)
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToUpdate);
        }
        var resultDto = new RecipeSummaryDto(
            Id: recipeEntity.Id,
            Name: recipeEntity.Name,
            Description: recipeEntity.Description,
            Ranking: recipeEntity.Ranking,
            OriginalSource: recipeEntity.OriginalSource,
            CookTime: recipeEntity.CookTime,
            PrepTime: recipeEntity.PrepTime,
            Servings: recipeEntity.Servings,
            OwnerUserId: recipeEntity.OwnerUserId,
            Visibility: recipeEntity.Visibility.ToDtoVisibility()
        );
        return Result<RecipeSummaryDto>.Success(resultDto);
    }

    public async Task<Result<RecipeDto>> GetRecipeByIdAsync(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
        {
            return Result<RecipeDto>.Failure(RecipeErrors.NotFound);
        }
        var recipeDto = new RecipeDto(
            Id: recipe.Id,
            Name: recipe.Name,
            Description: recipe.Description,
            Notes: recipe.Notes,
            Ranking: recipe.Ranking,
            OriginalSource: recipe.OriginalSource,
            CookTime: recipe.CookTime,
            PrepTime: recipe.PrepTime,
            Servings: recipe.Servings,
            OwnerUserId: recipe.OwnerUserId,
            Visibility: recipe.Visibility.ToDtoVisibility(),
            Ingredients: null,
            Instructions: null
        );
        return Result<RecipeDto>.Success(recipeDto);
    }

    public async Task<Result<IEnumerable<RecipeSummaryDto>>> GetRecipesByOwnerIdAsync(int userId)
    {
        var recipes = await _recipeRepository.GetByOwnerIdAsync(userId);
        var recipeDtos = recipes.Select(r => new RecipeSummaryDto(
            Id: r.Id,
            Name: r.Name,
            Description: r.Description,
            Ranking: r.Ranking,
            OriginalSource: r.OriginalSource,
            CookTime: r.CookTime,
            PrepTime: r.PrepTime,
            Servings: r.Servings,
            OwnerUserId: r.OwnerUserId,
            Visibility: r.Visibility.ToDtoVisibility()
        ));
        return Result<IEnumerable<RecipeSummaryDto>>.Success(recipeDtos);
    }
    #endregion

    #region Recipe ingredient operations
    public async Task<Result<RecipeIngredientDto>> AddIngredientToRecipeAsync(RecipeIngredientDto ingredient)
    {
        var newIngredient = new RecipeIngredient
        {
            RecipeId = ingredient.RecipeId,
            Name = ingredient.Name,
            Amount = ingredient.Amount,
            MeasurementType = ingredient.MeasurementType
        };

        var createdIngredient = await _ingredientRepository.CreateAsync(newIngredient);
        if (createdIngredient == null)
        {
            return Result<RecipeIngredientDto>.Failure(RecipeIngredientErrors.UnableToCreate);
        }

        var resultDto = new RecipeIngredientDto(createdIngredient.Id, createdIngredient.RecipeId, createdIngredient.Name, createdIngredient.Amount, createdIngredient.MeasurementType);
        return Result<RecipeIngredientDto>.Success(resultDto);
    }
    public async Task<Result<IEnumerable<RecipeIngredientDto>>> GetIngredientsByRecipeIdAsync(int recipeId)
    {
        var ingredients = await _ingredientRepository.GetByRecipeIdAsync(recipeId);
        var ingredientDtos = ingredients.Select(i => new RecipeIngredientDto(i.Id, i.RecipeId, i.Name, i.Amount, i.MeasurementType));
        return Result<IEnumerable<RecipeIngredientDto>>.Success(ingredientDtos);
    }


    public async Task<Result<RecipeIngredientDto>> UpdateRecipeIngredientAsync(RecipeIngredientDto ingredient)
    {
        var ingredientEntity = new RecipeIngredient
        {
            Id = ingredient.Id,
            RecipeId = ingredient.RecipeId,
            Name = ingredient.Name,
            Amount = ingredient.Amount,
            MeasurementType = ingredient.MeasurementType
        };

        var updateResult = await _ingredientRepository.UpdateAsync(ingredientEntity);
        if (!updateResult)
        {
            return Result<RecipeIngredientDto>.Failure(RecipeIngredientErrors.UnableToUpdate);
        }
        var resultDto = new RecipeIngredientDto(ingredientEntity.Id, ingredientEntity.RecipeId, ingredientEntity.Name, ingredientEntity.Amount, ingredientEntity.MeasurementType);
        return Result<RecipeIngredientDto>.Success(resultDto);
    }
    public async Task<Result<bool>> DeleteRecipeIngredientAsync(int id)
    {
        var deleteResult = await _ingredientRepository.DeleteAsync(id);
        if (!deleteResult)
        {
            return Result<bool>.Failure(RecipeIngredientErrors.UnableToDelete);
        }
        return Result<bool>.Success(deleteResult);
    }


    #endregion

    #region Recipe instruction operations
    public async Task<Result<RecipeInstructionDto>> AddInstructionToRecipeAsync(RecipeInstructionDto instruction)
    {
        var newInstruction = new RecipeInstruction
        {
            RecipeId = instruction.RecipeId,
            StepNumber = instruction.StepNumber,
            Description = instruction.Description,
            Note = instruction.Note
        };

        var createdInstruction = await _instructionRepository.CreateAsync(newInstruction);
        if (createdInstruction == null)
        {
            return Result<RecipeInstructionDto>.Failure(RecipeInstructionErrors.UnableToCreate);
        }
        var resultDto = new RecipeInstructionDto(createdInstruction.Id, createdInstruction.RecipeId, createdInstruction.StepNumber, createdInstruction.Description, createdInstruction.Note);
        return Result<RecipeInstructionDto>.Success(resultDto);

    }
    public async Task<Result<IEnumerable<RecipeInstructionDto>>> GetInstructionsByRecipeIdAsync(int recipeId)
    {
        var instructions = await _instructionRepository.GetByRecipeIdAsync(recipeId);
        if (instructions == null || !instructions.Any())
        {
            return Result<IEnumerable<RecipeInstructionDto>>.Failure(RecipeInstructionErrors.NotFound);
        }
        var instructionDtos = instructions.Select(i => new RecipeInstructionDto(i.Id, i.RecipeId, i.StepNumber, i.Description, i.Note));
        return Result<IEnumerable<RecipeInstructionDto>>.Success(instructionDtos);
    }
    public async Task<Result<RecipeInstructionDto>> UpdateRecipeInstructionAsync(RecipeInstructionDto instruction)
    {
        var instructionEntity = new RecipeInstruction
        {
            Id = instruction.Id,
            RecipeId = instruction.RecipeId,
            StepNumber = instruction.StepNumber,
            Description = instruction.Description,
            Note = instruction.Note
        };

        var updateResult = await _instructionRepository.UpdateAsync(instructionEntity);
        if (!updateResult)
        {
            return Result<RecipeInstructionDto>.Failure(RecipeInstructionErrors.UnableToUpdate);
        }
        var resultDto = new RecipeInstructionDto(instructionEntity.Id, instructionEntity.RecipeId, instructionEntity.StepNumber, instructionEntity.Description, instructionEntity.Note);
        return Result<RecipeInstructionDto>.Success(resultDto);
    }
    public async Task<Result<bool>> DeleteRecipeInstructionAsync(int id)
    {
        var deleteResult = await _instructionRepository.DeleteAsync(id);
        if (!deleteResult)
        {
            return Result<bool>.Failure(RecipeInstructionErrors.UnableToDelete);
        }
        return Result<bool>.Success(deleteResult);
    }
    #endregion








    #region Recipe share operations

    public async Task<Result<RecipeShareDto>> CreateShareAsync(RecipeShareCreateDto share)
    {
        var shareEntity = new RecipeShare
        {
            RecipeId = share.RecipeId,
            SharedWithUserId = share.SharedWithUserId,
            SharedWithGroupId = share.SharedWithGroupId,
            SharedByUserId = share.SharedByUserId,
            Permission = share.Permission.ToEntityPermission(),
        };
        var createdShare = await _shareRepository.CreateAsync(shareEntity);
        if (createdShare == null)
        {
            return Result<RecipeShareDto>.Failure(RecipeErrors.UnableToCreate);
        }
        var shareDto = new RecipeShareDto(
            Id: createdShare.Id,
            RecipeId: createdShare.RecipeId ?? 0,
            SharedWithUserId: createdShare.SharedWithUserId ?? 0,
            SharedWithGroupId: createdShare.SharedWithGroupId ?? 0,
            SharedByUserId: createdShare.SharedByUserId ?? 0,
            Permission: createdShare.Permission.ToDtoPermission(),
            SharedAt: createdShare.CreatedAt
        );
        return Result<RecipeShareDto>.Success(shareDto);
    }
    public async Task<Result<IEnumerable<RecipeShareDto>>> GetSharesBySharedWithGroupIdAsync(int groupId)
    {
        var shares = await _shareRepository.GetBySharedWithGroupIdAsync(groupId);
        var shareDtos = shares.Select(s => new RecipeShareDto(
            Id: s.Id,
            RecipeId: s.RecipeId ?? 0,
            SharedWithUserId: s.SharedWithUserId ?? 0,
            SharedWithGroupId: s.SharedWithGroupId ?? 0,
            SharedByUserId: s.SharedByUserId ?? 0,
            Permission: s.Permission.ToDtoPermission(),
            SharedAt: s.CreatedAt
        ));
        return Result<IEnumerable<RecipeShareDto>>.Success(shareDtos);
    }

    public async Task<Result<IEnumerable<RecipeShareDto>>> GetSharesBySharedWithUserIdAsync(int userId)
    {
        var shares = await _shareRepository.GetBySharedWithUserIdAsync(userId);
        var shareDtos = shares.Select(s => new RecipeShareDto(
            Id: s.Id,
            RecipeId: s.RecipeId ?? 0,
            SharedWithUserId: s.SharedWithUserId ?? 0,
            SharedWithGroupId: s.SharedWithGroupId ?? 0,
            SharedByUserId: s.SharedByUserId ?? 0,
            Permission: s.Permission.ToDtoPermission(),
            SharedAt: s.CreatedAt
        ));
        return Result<IEnumerable<RecipeShareDto>>.Success(shareDtos);
    }
    public async Task<Result<RecipeShareDto>> GetShareByIdAsync(int id)
    {
        var share = await _shareRepository.GetByIdAsync(id);
        if (share == null)
        {
            return Result<RecipeShareDto>.Failure(RecipeShareErrors.NotFound);
        }
        var shareDto = new RecipeShareDto(
            Id: share.Id,
            RecipeId: share.RecipeId ?? 0,
            SharedWithUserId: share.SharedWithUserId ?? 0,
            SharedWithGroupId: share.SharedWithGroupId ?? 0,
            SharedByUserId: share.SharedByUserId ?? 0,
            Permission: share.Permission.ToDtoPermission(),
            SharedAt: share.CreatedAt
        );
        return Result<RecipeShareDto>.Success(shareDto);
    }

    public async Task<Result<IEnumerable<RecipeShareDto>>> GetShareByRecipeIdAsync(int recipeId)
    {
        var shares = await _shareRepository.GetByRecipeIdAsync(recipeId);
        var shareDtos = shares.Select(s => new RecipeShareDto(
            Id: s.Id,
            RecipeId: s.RecipeId ?? 0,
            SharedWithUserId: s.SharedWithUserId ?? 0,
            SharedWithGroupId: s.SharedWithGroupId ?? 0,
            SharedByUserId: s.SharedByUserId ?? 0,
            Permission: s.Permission.ToDtoPermission(),
            SharedAt: s.CreatedAt
        ));

        return Result<IEnumerable<RecipeShareDto>>.Success(shareDtos);
    }
    public async Task<Result<RecipeShareDto>> UpdateShareAsync(RecipeShareUpdateDto share)
    {
        var shareEntity = new RecipeShare
        {
            Id = share.Id,
            RecipeId = share.RecipeId,
            SharedWithUserId = share.SharedWithUserId,
            SharedWithGroupId = share.SharedWithGroupId,
            SharedByUserId = share.SharedByUserId,
            Permission = share.Permission.ToEntityPermission(),
        };

        var updateResult = await _shareRepository.UpdateAsync(shareEntity);
        if (!updateResult)
        {
            return Result<RecipeShareDto>.Failure(RecipeShareErrors.UnableToUpdate);
        }
        var resultDto = new RecipeShareDto(
            Id: shareEntity.Id,
            RecipeId: shareEntity.RecipeId ?? 0,
            SharedWithUserId: shareEntity.SharedWithUserId ?? 0,
            SharedWithGroupId: shareEntity.SharedWithGroupId ?? 0,
            SharedByUserId: shareEntity.SharedByUserId ?? 0,
            Permission: shareEntity.Permission.ToDtoPermission(),
            SharedAt: shareEntity.CreatedAt
        );
        return Result<RecipeShareDto>.Success(resultDto);
    }

    public async Task<Result<bool>> DeleteShareAsync(int id)
    {
        var deleteResult = await _shareRepository.DeleteAsync(id);
        if (!deleteResult)
        {
            return Result<bool>.Failure(RecipeShareErrors.UnableToDelete);
        }
        return Result<bool>.Success(deleteResult);
    }
    #endregion
}