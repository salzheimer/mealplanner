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
    //private readonly IRecipeShareRepository _shareRepository;

    public RecipeService(IRecipeRepository recipeRepository, IRecipeIngredientRepository ingredientRepository, IRecipeInstructionRepository instructionRepository, IRecipeShareRepository shareRepository)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _instructionRepository = instructionRepository;
       // _shareRepository = shareRepository;
    }

    #region Recipe operations

    public async Task<Result<RecipeSummaryDto>> CreateRecipeAsync(int userId, RecipeCreateDto recipe)
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
            OwnerUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedBy = userId
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
            OwnerUserId: createdRecipe.OwnerUserId

         );
        return Result<RecipeSummaryDto>.Success(recipeDto);
    }

    public async Task<Result<bool>> DeleteRecipeAsync(int userId, int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
        {
            return Result<bool>.Failure(RecipeErrors.NotFound);
        }
        if (recipe.OwnerUserId != userId)
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }

        var deleted = await _recipeRepository.DeleteAsync(recipe.Id);
        if (!deleted)
        {
            return Result<bool>.Failure(RecipeErrors.UnableToDelete);
        }
        return Result<bool>.Success(true);
    }
    /// <summary>
    /// This method returns all recipes that the user has access to, including both owned and shared recipes. 
    /// It first retrieves the recipes owned by the user, then retrieves the recipes shared with the user directly, and finally retrieves the recipes shared with any groups that the user is a member of. The results are combined and returned as a list of RecipeSummaryDto objects.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<Result<IEnumerable<RecipeSummaryDto>>> GetAllRecipesAsync(int userId)
    {

        IEnumerable<Recipe> recipes;
        recipes = await _recipeRepository.GetByOwnerIdAsync(userId);

        //Get recipes shared with user directly
        // var sharedRecipes = await _shareRepository.GetBySharedWithUserIdAsync(userId);
        // var sharedRecipeIds = sharedRecipes.Select(s => s.RecipeId).ToHashSet();
        // var sharedRecipesEntities = await _recipeRepository.GetByIdsAsync(sharedRecipeIds);
        // recipes = recipes.Concat(sharedRecipesEntities);
        //Get recipes shared with groups user is a member of

        var recipeDtos = recipes.Select(r => new RecipeSummaryDto(
            Id: r.Id,
            Name: r.Name,
            Description: r.Description,
            Ranking: r.Ranking,
            OriginalSource: r.OriginalSource,
            CookTime: r.CookTime,
            PrepTime: r.PrepTime,
            Servings: r.Servings,
            OwnerUserId: r.OwnerUserId
        ));
        return Result<IEnumerable<RecipeSummaryDto>>.Success(recipeDtos);
    }

    public async Task<Result<RecipeSummaryDto>> UpdateRecipeAsync(int userId, RecipeUpdateDto recipe)
    {
        if (!await UserHasAccessToRecipe(userId, recipe.Id))
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        if (!await UserHasEditAccessToRecipe(userId, recipe.Id))
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }

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
             UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
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
            OwnerUserId: recipeEntity.OwnerUserId
        );
        return Result<RecipeSummaryDto>.Success(resultDto);
    }

    public async Task<Result<RecipeDto>> GetRecipeByIdAsync(int userId, int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
        {
            return Result<RecipeDto>.Failure(RecipeErrors.NotFound);
        }
        if (!await UserHasAccessToRecipe(userId, recipe.Id))
        {
            return Result<RecipeDto>.Failure(RecipeErrors.Unauthorized);
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
            OwnerUserId: r.OwnerUserId
        ));
        return Result<IEnumerable<RecipeSummaryDto>>.Success(recipeDtos);
    }

    public async Task<Result<RecipeSummaryDto>> CloneRecipeAsync(int userId, int recipeId)
    {
        var userHasAccess = await UserHasAccessToRecipe(userId, recipeId);
        if (!userHasAccess)
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        var recipeResult = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipeResult == null)
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.NotFound);
        }
        var recipe = recipeResult;
        var newRecipeEntity = new Recipe
        {
            Name = recipe.Name + " (Copy)",
            Description = recipe.Description,
            Notes = recipe.Notes,
            Ranking = recipe.Ranking,
            OriginalSource = recipe.OriginalSource,
            CookTime = recipe.CookTime,
            PrepTime = recipe.PrepTime,
            Servings = recipe.Servings,
            OwnerUserId = userId,
            Visibility = Models.Visibility.Private,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedBy = userId
        };
        var createdRecipe = await _recipeRepository.CreateAsync(newRecipeEntity);
        if (createdRecipe == null)
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToCreate);
        }
        //clone ingredients
        var ingredients = await _ingredientRepository.GetByRecipeIdAsync(recipeId);
        if (ingredients != null && ingredients.Any())
        {
            foreach (var ingredient in ingredients)
            {
                var newIngredient = new RecipeIngredient
                {
                    RecipeId = createdRecipe.Id,
                    Name = ingredient.Name,
                    Amount = ingredient.Amount,
                    MeasurementType = ingredient.MeasurementType,
                    Note = ingredient.Note,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedBy = userId

                };
                await _ingredientRepository.CreateAsync(newIngredient);
            }
        }
        //clone instructions
        var instructions = await _instructionRepository.GetByRecipeIdAsync(recipeId);
        if (instructions != null && instructions.Any())
        {
            foreach (var instruction in instructions)
            {
                var newInstruction = new RecipeInstruction
                {
                    RecipeId = createdRecipe.Id,
                    StepNumber = instruction.StepNumber,
                    Description = instruction.Description,
                    Note = instruction.Note,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = userId,
                    UpdatedBy = userId
                };
                await _instructionRepository.CreateAsync(newInstruction);
            }
        }

        var resultDto = new RecipeSummaryDto(
            Id: createdRecipe.Id,
            Name: createdRecipe.Name,
            Description: createdRecipe.Description,
            Ranking: createdRecipe.Ranking,
            OriginalSource: createdRecipe.OriginalSource,
            CookTime: createdRecipe.CookTime,
            PrepTime: createdRecipe.PrepTime,
            Servings: createdRecipe.Servings,
            OwnerUserId: createdRecipe.OwnerUserId 
        );

        return Result<RecipeSummaryDto>.Success(resultDto);

    }
    #endregion

    #region Recipe ingredient operations
    public async Task<Result<RecipeIngredientSummaryDto>> AddIngredientToRecipeAsync(int userId, RecipeIngredientCreateDto ingredient)
    {
        if (!await UserHasAccessToRecipe(userId, ingredient.RecipeId))
        {
            return Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        if (!await UserHasEditAccessToRecipe(userId, ingredient.RecipeId))
        {
            return Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        var newIngredient = new RecipeIngredient
        {
            RecipeId = ingredient.RecipeId,
            Name = ingredient.Name,
            Amount = ingredient.Amount,
            MeasurementType = ingredient.MeasurementType,
            Note = ingredient.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        var createdIngredient = await _ingredientRepository.CreateAsync(newIngredient);
        if (createdIngredient == null)
        {
            return Result<RecipeIngredientSummaryDto>.Failure(RecipeIngredientErrors.UnableToCreate);
        }

        var resultDto = new RecipeIngredientSummaryDto(createdIngredient.Id, createdIngredient.RecipeId, createdIngredient.Name, createdIngredient.Amount, createdIngredient.MeasurementType, createdIngredient.Note);
        return Result<RecipeIngredientSummaryDto>.Success(resultDto);
    }
    public async Task<Result<IEnumerable<RecipeIngredientSummaryDto>>> GetIngredientsByRecipeIdAsync(int userId, int recipeId)
    {
        if (!await UserHasAccessToRecipe(userId, recipeId))
        {
            return Result<IEnumerable<RecipeIngredientSummaryDto>>.Failure(RecipeErrors.Unauthorized);
        }

        var ingredients = await _ingredientRepository.GetByRecipeIdAsync(recipeId);
        var ingredientDtos = ingredients.Select(i => new RecipeIngredientSummaryDto(i.Id, i.RecipeId, i.Name, i.Amount, i.MeasurementType, i.Note));
        return Result<IEnumerable<RecipeIngredientSummaryDto>>.Success(ingredientDtos);
    }


    public async Task<Result<RecipeIngredientSummaryDto>> UpdateRecipeIngredientAsync(int userId, RecipeIngredientSummaryDto ingredient)
    {
        if (!await UserHasAccessToRecipe(userId, ingredient.RecipeId))
        {
            return Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        if (!await UserHasEditAccessToRecipe(userId, ingredient.RecipeId))
        {
            return Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        var ingredientEntity = new RecipeIngredient
        {
            Id = ingredient.Id,
            RecipeId = ingredient.RecipeId,
            Name = ingredient.Name,
            Amount = ingredient.Amount,
            MeasurementType = ingredient.MeasurementType,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        var updateResult = await _ingredientRepository.UpdateAsync(ingredientEntity);
        if (!updateResult)
        {
            return Result<RecipeIngredientSummaryDto>.Failure(RecipeIngredientErrors.UnableToUpdate);
        }
        var resultDto = new RecipeIngredientSummaryDto(ingredientEntity.Id, ingredientEntity.RecipeId, ingredientEntity.Name, ingredientEntity.Amount, ingredientEntity.MeasurementType, ingredientEntity.Note);
        return Result<RecipeIngredientSummaryDto>.Success((RecipeIngredientSummaryDto)resultDto);
    }
    public async Task<Result<bool>> DeleteRecipeIngredientAsync(int userId, int id)
    {
        if (!await UserHasAccessToRecipe(userId, id))
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }
        if (!await UserHasEditAccessToRecipe(userId, id))
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }

        var deleteResult = await _ingredientRepository.DeleteAsync(id);
        if (!deleteResult)
        {
            return Result<bool>.Failure(RecipeIngredientErrors.UnableToDelete);
        }
        return Result<bool>.Success(deleteResult);
    }


    #endregion

    #region Recipe instruction operations
    public async Task<Result<RecipeInstructionDto>> AddInstructionToRecipeAsync(int userId, RecipeInstructionCreateDto instruction)
    {
        if (!await UserHasAccessToRecipe(userId, instruction.RecipeId))
        {
            return Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized);
        }
        if (!await UserHasEditAccessToRecipe(userId, instruction.RecipeId))
        {
            return Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized);
        }
        var newInstruction = new RecipeInstruction
        {
            RecipeId = instruction.RecipeId,
            StepNumber = instruction.StepNumber,
            Description = instruction.Description,
            Note = instruction.Note,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedBy = userId
        };

        var createdInstruction = await _instructionRepository.CreateAsync(newInstruction);
        if (createdInstruction == null)
        {
            return Result<RecipeInstructionDto>.Failure(RecipeInstructionErrors.UnableToCreate);
        }
        var resultDto = new RecipeInstructionDto(createdInstruction.Id, createdInstruction.RecipeId, createdInstruction.StepNumber, createdInstruction.Description, createdInstruction.Note);
        return Result<RecipeInstructionDto>.Success(resultDto);

    }
    public async Task<Result<IEnumerable<RecipeInstructionDto>>> GetInstructionsByRecipeIdAsync(int userId, int recipeId)
    {
        if (!await UserHasAccessToRecipe(userId, recipeId))
        {
            return Result<IEnumerable<RecipeInstructionDto>>.Failure(RecipeErrors.Unauthorized);
        }
        var instructions = await _instructionRepository.GetByRecipeIdAsync(recipeId);
        if (instructions == null || !instructions.Any())
        {
            return Result<IEnumerable<RecipeInstructionDto>>.Failure(RecipeInstructionErrors.NotFound);
        }
        var instructionDtos = instructions.Select(i => new RecipeInstructionDto(i.Id, i.RecipeId, i.StepNumber, i.Description, i.Note));
        return Result<IEnumerable<RecipeInstructionDto>>.Success(instructionDtos);
    }
    public async Task<Result<RecipeInstructionDto>> UpdateRecipeInstructionAsync(int userId, RecipeInstructionDto instruction)
    {
        if (!await UserHasAccessToRecipe(userId, instruction.RecipeId))
        {
            return Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized);
        }
        if (!await UserHasEditAccessToRecipe(userId, instruction.RecipeId))
        {
            return Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized);
        }

        var instructionEntity = new RecipeInstruction
        {
            Id = instruction.Id,
            RecipeId = instruction.RecipeId,
            StepNumber = instruction.StepNumber,
            Description = instruction.Description,
            Note = instruction.Note,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        var updateResult = await _instructionRepository.UpdateAsync(instructionEntity);
        if (!updateResult)
        {
            return Result<RecipeInstructionDto>.Failure(RecipeInstructionErrors.UnableToUpdate);
        }
        var resultDto = new RecipeInstructionDto(instructionEntity.Id, instructionEntity.RecipeId, instructionEntity.StepNumber, instructionEntity.Description, instructionEntity.Note);
        return Result<RecipeInstructionDto>.Success(resultDto);
    }
    public async Task<Result<bool>> DeleteRecipeInstructionAsync(int userId, int id)
    {
        if (!await UserHasAccessToRecipe(userId, id))
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }
        if (!await UserHasEditAccessToRecipe(userId, id))
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }
        var deleteResult = await _instructionRepository.DeleteAsync(id);
        if (!deleteResult)
        {
            return Result<bool>.Failure(RecipeInstructionErrors.UnableToDelete);
        }
        return Result<bool>.Success(deleteResult);
    }
    #endregion







/*
    #region Recipe share operations

    public async Task<Result<RecipeShareDto>> CreateShareAsync(int userId, RecipeShareCreateDto share)
    {
        var recipe = await _recipeRepository.GetByIdAsync(share.RecipeId);
        if (recipe == null)
        {
            return Result<RecipeShareDto>.Failure(RecipeErrors.NotFound);
        }
        if (recipe.OwnerUserId != userId)
        {
            return Result<RecipeShareDto>.Failure(RecipeErrors.Unauthorized);
        }

        var shareEntity = new RecipeShare
        {
            RecipeId = share.RecipeId,
            SharedWithUserId = share.SharedWithUserId,
            SharedWithGroupId = share.SharedWithGroupId,
            SharedByUserId = userId,
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

    public async Task<Result<IEnumerable<RecipeShareDto>>> GetShareByRecipeIdAsync(int userId, int recipeId)
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
    public async Task<Result<RecipeShareDto>> UpdateShareAsync(int userId, RecipeShareUpdateDto share)
    {
        var existingShare = await _shareRepository.GetByIdAsync(share.Id);
        if (existingShare == null)
        {
            return Result<RecipeShareDto>.Failure(RecipeShareErrors.NotFound);
        }
        if (existingShare.SharedByUserId != userId)
        {
            return Result<RecipeShareDto>.Failure(RecipeShareErrors.Unauthorized);
        }
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

    public async Task<Result<bool>> DeleteShareAsync(int userId, int id)
    {
        var share = await _shareRepository.GetByIdAsync(id);
        if (share == null)
        {
            return Result<bool>.Failure(RecipeShareErrors.NotFound);
        }
        if (share.SharedByUserId != userId && share.SharedWithUserId != userId) //allow either the user who shared or the user it was shared with to delete the share
        {
            return Result<bool>.Failure(RecipeShareErrors.Unauthorized);
        }
        var deleteResult = await _shareRepository.DeleteAsync(id);
        if (!deleteResult)
        {
            return Result<bool>.Failure(RecipeShareErrors.UnableToDelete);
        }
        return Result<bool>.Success(deleteResult);
    }
    #endregion

*/
    #region private helper methods
    /// <summary>
    /// Checks if user has access to recipe either through ownership or sharing. This is used to enforce authorization rules for recipe access and modification.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="recipeId"></param>
    /// <returns>boolean indicating if user has access</returns>
    private async Task<bool> UserHasAccessToRecipe(int userId, int recipeId)
    {
        //TODO: optimize by checking for share before fetching shares for recipe
        //TODO: also need to check for group shares and if user is in group, but skipping for now to get basic sharing functionality working
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
        {
            return false;
        }
        if (recipe.OwnerUserId == userId)
        {
            return true;
        }
        // var shares = await _shareRepository.GetByRecipeIdAsync(recipeId);
        // var hasAccess = shares.Any(s => s.SharedWithUserId == userId);
        return false;
    }
    private async Task<bool> UserHasEditAccessToRecipe(int userId, int recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
        {
            return false;
        }
        if (recipe.OwnerUserId == userId)
        {
            return true;
        }
    //    var shares = await _shareRepository.GetByRecipeIdAsync(recipeId);
    //      var hasAccess = shares.Any(s => s.SharedWithUserId == userId && s.Permission == Models.Permission.Edit);
        return false;
    }

    #endregion
}