using MealRecipeService.Clients;
using MealRecipeService.Contracts;
using MealRecipeService.Models;
using MealRecipeService.Interfaces;
using Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace MealRecipeService.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeIngredientRepository _ingredientRepository;
    private readonly IRecipeInstructionRepository _instructionRepository;
    private readonly IAccessService _accessService;

    public RecipeService(IRecipeRepository recipeRepository, IRecipeIngredientRepository ingredientRepository, IRecipeInstructionRepository instructionRepository, IAccessService accessService)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _instructionRepository = instructionRepository;
        _accessService =accessService;
    }

    #region Recipe operations

    public async Task<Result<RecipeSummaryResponse>> CreateRecipeAsync(Guid userId, CreateRecipeRequest recipe)
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
            return Result<RecipeSummaryResponse>.Failure(RecipeErrors.UnableToCreate);
        }
        var recipeDto = new RecipeSummaryResponse(
            Id: createdRecipe.Id,
            Name: createdRecipe.Name,
            Description: createdRecipe.Description,
            Notes: createdRecipe.Notes,
            Ranking: createdRecipe.Ranking,
            OriginalSource: createdRecipe.OriginalSource,
            CookTime: createdRecipe.CookTime,
            PrepTime: createdRecipe.PrepTime,
            Servings: createdRecipe.Servings,
            OwnerUserId: createdRecipe.OwnerUserId
        );
        return Result<RecipeSummaryResponse>.Success(recipeDto);
    }

    public async Task<Result<bool>> DeleteRecipeAsync(Guid userId, Guid recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
        {
            return Result<bool>.Failure(RecipeErrors.NotFound);
        }
        if (!await UserHasEditAccessToRecipe(userId, recipeId))
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }

        var deleted = await _recipeRepository.DeleteAsync(recipe.Id);
        if (!deleted)
        {
            return Result<bool>.Failure(RecipeErrors.UnableToDelete);
        }
        //TODO: delete associated ingredients and instructions
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<RecipeSummaryResponse>>> GetAllRecipesAsync(Guid userId)
    {
        var recipes = await _recipeRepository.GetByOwnerIdAsync(userId);

        //get recipes shared with user
        var sharedPermissions = await _accessService.GetRecipesSharedWithUser(userId);
        var sharedRecipeIds = sharedPermissions.IsSuccess
            ? sharedPermissions.Value!.Select(p => p.ResourceId).ToHashSet()
            : new HashSet<Guid>();

        var sharedRecipes = sharedRecipeIds.Any() ? await _recipeRepository.GetByIdsAsync(sharedRecipeIds) : Enumerable.Empty<Recipe>();
        recipes = recipes.Concat(sharedRecipes);


        var recipeDtos = recipes.Select(r => new RecipeSummaryResponse(
            Id: r.Id,
            Name: r.Name,
            Description: r.Description,
            Notes: r.Notes,
            Ranking: r.Ranking,
            OriginalSource: r.OriginalSource,
            CookTime: r.CookTime,
            PrepTime: r.PrepTime,
            Servings: r.Servings,
            OwnerUserId: r.OwnerUserId
        ));
        return Result<IEnumerable<RecipeSummaryResponse>>.Success(recipeDtos);
    }
    public async Task<Result<IEnumerable<RecipeSummaryResponse>>> GetRecipesSharedWithMeAsync(Guid userId)
    {

        //get recipes shared with user
        var sharedPermissions = await _accessService.GetRecipesSharedWithUser(userId);
        var sharedRecipeIds = sharedPermissions.IsSuccess
            ? sharedPermissions.Value!.Select(p => p.ResourceId).ToHashSet()
            : new HashSet<Guid>();

        var sharedRecipes = sharedRecipeIds.Any() ? await _recipeRepository.GetByIdsAsync(sharedRecipeIds) : Enumerable.Empty<Recipe>();



        var recipeDtos = sharedRecipes.Select(r => new RecipeSummaryResponse(
            Id: r.Id,
            Name: r.Name,
            Description: r.Description,
            Notes: r.Notes,
            Ranking: r.Ranking,
            OriginalSource: r.OriginalSource,
            CookTime: r.CookTime,
            PrepTime: r.PrepTime,
            Servings: r.Servings,
            OwnerUserId: r.OwnerUserId
        ));
        return Result<IEnumerable<RecipeSummaryResponse>>.Success(recipeDtos);
    }
    public async Task<Result<RecipeSummaryResponse>> UpdateRecipeAsync(Guid userId, Guid recipeId,UpdateRecipeRequest recipe)
    {
        if (!await UserHasEditAccessToRecipe(userId, recipeId))
        {
            return Result<RecipeSummaryResponse>.Failure(RecipeErrors.Unauthorized);
        }

        var recipeEntity = new Recipe
        {
            Id = recipeId,
            Name = recipe.Name,
            Description = recipe.Description,
            Notes = recipe.Notes,
            Ranking = recipe.Ranking,
            OriginalSource = recipe.OriginalSource,
            CookTime = recipe.CookTime,
            PrepTime = recipe.PrepTime,
            Servings = recipe.Servings,
            OwnerUserId = recipe.OwnerUserId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        var updatedRecipe = await _recipeRepository.UpdateAsync(recipeEntity);
        if (!updatedRecipe)
        {
            return Result<RecipeSummaryResponse>.Failure(RecipeErrors.UnableToUpdate);
        }
        var resultDto = new RecipeSummaryResponse(
            Id: recipeEntity.Id,
            Name: recipeEntity.Name,
            Description: recipeEntity.Description,
            Notes: recipeEntity.Notes,
            Ranking: recipeEntity.Ranking,
            OriginalSource: recipeEntity.OriginalSource,
            CookTime: recipeEntity.CookTime,
            PrepTime: recipeEntity.PrepTime,
            Servings: recipeEntity.Servings,
            OwnerUserId: recipeEntity.OwnerUserId
        );
        return Result<RecipeSummaryResponse>.Success(resultDto);
    }

    public async Task<Result<RecipeSummaryResponse>> GetRecipeByIdAsync(Guid userId, Guid id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
        {
            return Result<RecipeSummaryResponse>.Failure(RecipeErrors.NotFound);
        }
        if (!await UserHasAccessToRecipe(userId, recipe.Id))
        {
            return Result<RecipeSummaryResponse>.Failure(RecipeErrors.Unauthorized);
        }

        var recipeDto = new RecipeSummaryResponse(
            Id: recipe.Id,
            Name: recipe.Name,
            Description: recipe.Description,
            Notes: recipe.Notes,
            Ranking: recipe.Ranking,
            OriginalSource: recipe.OriginalSource,
            CookTime: recipe.CookTime,
            PrepTime: recipe.PrepTime,
            Servings: recipe.Servings,
            OwnerUserId: recipe.OwnerUserId

        );
        return Result<RecipeSummaryResponse>.Success(recipeDto);
    }

    public async Task<Result<RecipeDetailResponse>> GetRecipeDetailAsync(Guid userId, Guid recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
        {
            return Result<RecipeDetailResponse>.Failure(RecipeErrors.NotFound);
        }
        if (!await UserHasAccessToRecipe(userId, recipe.Id))
        {
            return Result<RecipeDetailResponse>.Failure(RecipeErrors.Unauthorized);
        }

        var ingredients = await GetIngredientsByRecipeIdAsync(userId, recipe.Id);

        var instructions = await GetInstructionsByRecipeIdAsync(userId, recipe.Id);

        var recipeDto = new RecipeDetailResponse(
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
            Ingredients: ingredients.IsSuccess ? ingredients.Value : null,
            Instructions: instructions.IsSuccess ? instructions.Value : null

        );

        return Result<RecipeDetailResponse>.Success(recipeDto);
    }
    public async Task<Result<IEnumerable<RecipeSummaryResponse>>> GetRecipesByOwnerIdAsync(Guid userId)
    {
        var recipes = await _recipeRepository.GetByOwnerIdAsync(userId);
        var recipeDtos = recipes.Select(r => new RecipeSummaryResponse(
            Id: r.Id,
            Name: r.Name,
            Description: r.Description,
            Notes: r.Notes,
            Ranking: r.Ranking,
            OriginalSource: r.OriginalSource,
            CookTime: r.CookTime,
            PrepTime: r.PrepTime,
            Servings: r.Servings,
            OwnerUserId: r.OwnerUserId
        ));
        return Result<IEnumerable<RecipeSummaryResponse>>.Success(recipeDtos);
    }

    public async Task<Result<RecipeSummaryResponse>> CloneRecipeAsync(Guid userId, Guid recipeId)
    {
        var userHasAccess = await UserHasAccessToRecipe(userId, recipeId);
        if (!userHasAccess)
        {
            return Result<RecipeSummaryResponse>.Failure(RecipeErrors.Unauthorized);
        }
        var recipeResult = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipeResult == null)
        {
            return Result<RecipeSummaryResponse>.Failure(RecipeErrors.NotFound);
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedBy = userId
        };
        var createdRecipe = await _recipeRepository.CreateAsync(newRecipeEntity);
        if (createdRecipe == null)
        {
            return Result<RecipeSummaryResponse>.Failure(RecipeErrors.UnableToCreate);
        }
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

        var resultDto = new RecipeSummaryResponse(
            Id: createdRecipe.Id,
            Name: createdRecipe.Name,
            Description: createdRecipe.Description,
            Notes: createdRecipe.Notes,
            Ranking: createdRecipe.Ranking,
            OriginalSource: createdRecipe.OriginalSource,
            CookTime: createdRecipe.CookTime,
            PrepTime: createdRecipe.PrepTime,
            Servings: createdRecipe.Servings,
            OwnerUserId: createdRecipe.OwnerUserId
        );

        return Result<RecipeSummaryResponse>.Success(resultDto);
    }
    #endregion

    #region Recipe ingredient operations
    public async Task<Result<RecipeIngredientSummaryResponse>> AddIngredientToRecipeAsync(Guid userId, Guid recipeId, CreateRecipeIngredientRequest ingredient)
    {
        if (!await UserHasEditAccessToRecipe(userId, recipeId))
        {
            return Result<RecipeIngredientSummaryResponse>.Failure(RecipeErrors.Unauthorized);
        }
        var newIngredient = new RecipeIngredient
        {
            RecipeId = recipeId,
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
            return Result<RecipeIngredientSummaryResponse>.Failure(RecipeIngredientErrors.UnableToCreate);
        }

        var resultDto = new RecipeIngredientSummaryResponse(createdIngredient.Id, createdIngredient.RecipeId, createdIngredient.Name, createdIngredient.Amount, createdIngredient.MeasurementType, createdIngredient.Note);
        return Result<RecipeIngredientSummaryResponse>.Success(resultDto);
    }
    public async Task<Result<IEnumerable<RecipeIngredientSummaryResponse>>> GetIngredientsByRecipeIdAsync(Guid userId, Guid recipeId)
    {
        if (!await UserHasAccessToRecipe(userId, recipeId))
        {
            return Result<IEnumerable<RecipeIngredientSummaryResponse>>.Failure(RecipeErrors.Unauthorized);
        }

        var ingredients = await _ingredientRepository.GetByRecipeIdAsync(recipeId);
        var ingredientDtos = ingredients.Select(i => new RecipeIngredientSummaryResponse(i.Id, i.RecipeId, i.Name, i.Amount, i.MeasurementType, i.Note));
        return Result<IEnumerable<RecipeIngredientSummaryResponse>>.Success(ingredientDtos);
    }

    public async Task<Result<RecipeIngredientSummaryResponse>> UpdateRecipeIngredientAsync(Guid userId, Guid ingredientId, Guid recipeId, UpdateRecipeIngredientRequest ingredient)
    {
        if (!await UserHasEditAccessToRecipe(userId, recipeId))
        {
            return Result<RecipeIngredientSummaryResponse>.Failure(RecipeErrors.Unauthorized);
        }
        var ingredientEntity = new RecipeIngredient
        {
            Id = ingredientId,
            RecipeId = recipeId,
            Name = ingredient.Name,
            Amount = ingredient.Amount,
            MeasurementType = ingredient.MeasurementType,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        var updateResult = await _ingredientRepository.UpdateAsync(ingredientEntity);
        if (!updateResult)
        {
            return Result<RecipeIngredientSummaryResponse>.Failure(RecipeIngredientErrors.UnableToUpdate);
        }
        var resultDto = new RecipeIngredientSummaryResponse(ingredientEntity.Id, ingredientEntity.RecipeId, ingredientEntity.Name, ingredientEntity.Amount, ingredientEntity.MeasurementType, ingredientEntity.Note);
        return Result<RecipeIngredientSummaryResponse>.Success(resultDto);
    }
    public async Task<Result<bool>> DeleteRecipeIngredientAsync(Guid userId, Guid recipeId, Guid ingredientId)
    {
        if (!await UserHasEditAccessToRecipe(userId, recipeId))
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }

        var deleteResult = await _ingredientRepository.DeleteAsync(ingredientId);
        if (!deleteResult)
        {
            return Result<bool>.Failure(RecipeIngredientErrors.UnableToDelete);
        }
        return Result<bool>.Success(deleteResult);
    }
    #endregion

    #region Recipe instruction operations
    
    public async Task<Result<RecipeInstructionResponse>> AddInstructionToRecipeAsync(Guid userId, Guid recipeId, CreateRecipeInstructionRequest instruction)
    {
        if (!await UserHasEditAccessToRecipe(userId, recipeId))
        {
            return Result<RecipeInstructionResponse>.Failure(RecipeErrors.Unauthorized);
        }
        var newInstruction = new RecipeInstruction
        {
            RecipeId = recipeId,
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
            return Result<RecipeInstructionResponse>.Failure(RecipeInstructionErrors.UnableToCreate);
        }
        var resultDto = new RecipeInstructionResponse(createdInstruction.Id, createdInstruction.RecipeId, createdInstruction.StepNumber, createdInstruction.Description, createdInstruction.Note);
        return Result<RecipeInstructionResponse>.Success(resultDto);
    }
    
    public async Task<Result<IEnumerable<RecipeInstructionResponse>>> GetInstructionsByRecipeIdAsync(Guid userId, Guid recipeId)
    {
        if (!await UserHasAccessToRecipe(userId, recipeId))
        {
            return Result<IEnumerable<RecipeInstructionResponse>>.Failure(RecipeErrors.Unauthorized);
        }
        var instructions = await _instructionRepository.GetByRecipeIdAsync(recipeId);
        if (instructions == null || !instructions.Any())
        {
            return Result<IEnumerable<RecipeInstructionResponse>>.Success(Enumerable.Empty<RecipeInstructionResponse>());
        }
        var instructionDtos = instructions.Select(i => new RecipeInstructionResponse(i.Id, i.RecipeId, i.StepNumber, i.Description, i.Note));
        return Result<IEnumerable<RecipeInstructionResponse>>.Success(instructionDtos);
    }
    
    public async Task<Result<RecipeInstructionResponse>> UpdateRecipeInstructionAsync(Guid userId, Guid recipeId, Guid instructionId, UpdateRecipeInstructionRequest instruction)
    {
        if (!await UserHasEditAccessToRecipe(userId, recipeId))
        {
            return Result<RecipeInstructionResponse>.Failure(RecipeErrors.Unauthorized);
        }

        var instructionEntity = new RecipeInstruction
        {
            Id = instructionId,
            RecipeId = recipeId,
            StepNumber = instruction.StepNumber,
            Description = instruction.Description,
            Note = instruction.Note,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        var updateResult = await _instructionRepository.UpdateAsync(instructionEntity);
        if (!updateResult)
        {
            return Result<RecipeInstructionResponse>.Failure(RecipeInstructionErrors.UnableToUpdate);
        }
        var resultDto = new RecipeInstructionResponse(instructionEntity.Id, instructionEntity.RecipeId, instructionEntity.StepNumber, instructionEntity.Description, instructionEntity.Note);
        return Result<RecipeInstructionResponse>.Success(resultDto);
    }
    public async Task<Result<bool>> DeleteRecipeInstructionAsync(Guid userId, Guid recipeId, Guid instructionId)
    {
        if (!await UserHasEditAccessToRecipe(userId, recipeId))
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }
        var deleteResult = await _instructionRepository.DeleteAsync(instructionId);
        if (!deleteResult)
        {
            return Result<bool>.Failure(RecipeInstructionErrors.UnableToDelete);
        }
        return Result<bool>.Success(deleteResult);
    }
    #endregion

    #region Recipe share operations
    public async Task<Result<ShareRecipeResponse>> ShareRecipeAsync(ShareRecipeRequest shareRecipeRequest)
    {
        var recipe = await _recipeRepository.GetByIdAsync(shareRecipeRequest.RecipeId);
        if (recipe == null)
            return Result<ShareRecipeResponse>.Failure(RecipeErrors.NotFound);
        if (recipe.OwnerUserId != shareRecipeRequest.GrantedBy)
            return Result<ShareRecipeResponse>.Failure(RecipeErrors.Unauthorized);
        
        var permissionRequest = new CreateResourcePermissionRequest(
        "recipe",
        shareRecipeRequest.RecipeId,
        shareRecipeRequest.SubjectTypeName,
        shareRecipeRequest.SubjectId,        
        shareRecipeRequest.SubjectTypeName == "group" ? "view" :shareRecipeRequest.PermissionTypeName,
        shareRecipeRequest.GrantedBy,
        shareRecipeRequest.ExpiresAt
        );
        var accessResponse = await _accessService.GrantAccessToResource(permissionRequest);
        if (accessResponse.IsSuccess == false)
            return Result<ShareRecipeResponse>.Failure(RecipeErrors.UnableToShare);

        var response = new ShareRecipeResponse(
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

        return Result<ShareRecipeResponse>.Success(response);
    }
    #endregion

    #region private helper methods
    private async Task<bool> UserHasAccessToRecipe(Guid userId, Guid recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null) return false;
        if (recipe.OwnerUserId == userId) return true;

        var permissions = await _accessService.GetRecipesSharedWithUser(userId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId);
    }

    private async Task<bool> UserHasEditAccessToRecipe(Guid userId, Guid recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null) return false;
        if (recipe.OwnerUserId == userId) return true;

        var permissions = await _accessService.GetRecipesSharedWithUser(userId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId && p.PermissionTypeName =="edit");
    }
    #endregion
}
