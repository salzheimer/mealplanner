using MealRecipeService.Clients;
using MealRecipeService.Mappings;
using MealRecipeService.Models;
using MealRecipeService.Interfaces;
using Shared.Models;

namespace MealRecipeService.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IRecipeIngredientRepository _ingredientRepository;
    private readonly IRecipeInstructionRepository _instructionRepository;
    private readonly IIdentityServiceClient _identityClient;

    public RecipeService(IRecipeRepository recipeRepository, IRecipeIngredientRepository ingredientRepository, IRecipeInstructionRepository instructionRepository, IIdentityServiceClient identityClient)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _instructionRepository = instructionRepository;
        _identityClient = identityClient;
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
        if (!await UserHasEditAccessToRecipe(userId, id))
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

    public async Task<Result<IEnumerable<RecipeSummaryDto>>> GetAllRecipesAsync(int userId)
    {
        var recipes = await _recipeRepository.GetByOwnerIdAsync(userId);

        var sharedPermissions = await _identityClient.GetPermissionsForResourceAsync(ResourceType.Recipe, 0);
        // TODO: query permissions by subject (userId) once subject-scoped endpoint is wired into client

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
            OwnerUserId = recipe.OwnerUserId,
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
        return Result<RecipeIngredientSummaryDto>.Success(resultDto);
    }
    public async Task<Result<bool>> DeleteRecipeIngredientAsync(int userId, int recipeId, int ingredientId)
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
    public async Task<Result<RecipeInstructionDto>> AddInstructionToRecipeAsync(int userId, RecipeInstructionCreateDto instruction)
    {
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
            return Result<IEnumerable<RecipeInstructionDto>>.Success(Enumerable.Empty<RecipeInstructionDto>());
        }
        var instructionDtos = instructions.Select(i => new RecipeInstructionDto(i.Id, i.RecipeId, i.StepNumber, i.Description, i.Note));
        return Result<IEnumerable<RecipeInstructionDto>>.Success(instructionDtos);
    }
    public async Task<Result<RecipeInstructionDto>> UpdateRecipeInstructionAsync(int userId, RecipeInstructionDto instruction)
    {
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
    public async Task<Result<bool>> DeleteRecipeInstructionAsync(int userId, int recipeId, int instructionId)
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
    public async Task<Result<ResourcePermissionDto>> ShareRecipeAsync(int userId, int recipeId, ShareRequestDto request)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
            return Result<ResourcePermissionDto>.Failure(RecipeErrors.NotFound);
        if (recipe.OwnerUserId != userId)
            return Result<ResourcePermissionDto>.Failure(RecipeErrors.Unauthorized);

        var createDto = new ResourcePermissionCreateDto(
            ResourceType: ResourceType.Recipe,
            ResourceId: recipeId,
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
    private async Task<bool> UserHasAccessToRecipe(int userId, int recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null) return false;
        if (recipe.OwnerUserId == userId) return true;

        var permissions = await _identityClient.GetPermissionsForResourceAsync(ResourceType.Recipe, recipeId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId);
    }

    private async Task<bool> UserHasEditAccessToRecipe(int userId, int recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null) return false;
        if (recipe.OwnerUserId == userId) return true;

        var permissions = await _identityClient.GetPermissionsForResourceAsync(ResourceType.Recipe, recipeId);
        return permissions.IsSuccess && permissions.Value!.Any(p => p.SubjectId == userId && p.Permission >= Shared.Models.Permission.Edit);
    }
    #endregion
}
