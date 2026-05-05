using MealRecipeService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MealRecipeService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : BaseController
{
    private readonly IRecipeService _recipeService;
    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    //Recipe endpoints
    [HttpGet]
    [Authorize]
    public async Task<Result<IEnumerable<RecipeSummaryDto>>> GetAll()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return Result<IEnumerable<RecipeSummaryDto>>.Failure(RecipeErrors.Unauthorized);
        }
        var recipes = await _recipeService.GetAllRecipesAsync(userId.Value);
        return recipes;
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<Result<RecipeDto>> GetFullRecipe(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return Result<RecipeDto>.Failure(RecipeErrors.Unauthorized);
        }

        var recipe = await _recipeService.GetRecipeByIdAsync(userId.Value, id);

        return recipe;
    }
    [HttpPost]
    [Authorize]
    public async Task<Result<RecipeSummaryDto>> Create(RecipeCreateDto recipe)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        var createdRecipe = await _recipeService.CreateRecipeAsync(authenticatedUserId.Value, recipe);
        if (!createdRecipe.IsSuccess)
        {
            return Result<RecipeSummaryDto>.Failure(createdRecipe.Error);
        }
        return createdRecipe;

    }
    [HttpPost("{recipeId:int}/clone")]
    [Authorize]
    public async Task<Result<RecipeSummaryDto>> Clone(int recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        var clonedRecipe = await _recipeService.CloneRecipeAsync(authenticatedUserId.Value, recipeId);
        if (!clonedRecipe.IsSuccess)
        {
            return Result<RecipeSummaryDto>.Failure(clonedRecipe.Error);
        }
        return clonedRecipe;
    }
    [HttpPut("update")]
    [Authorize]
    public async Task<Result<RecipeSummaryDto>> Update([FromBody] RecipeUpdateDto recipe)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        var updatedRecipe = await _recipeService.UpdateRecipeAsync(authenticatedUserId.Value, recipe);
        if (!updatedRecipe.IsSuccess)
        {
            return Result<RecipeSummaryDto>.Failure(updatedRecipe.Error);
        }
        return updatedRecipe;
    }
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<Result<bool>> Delete(int id)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<bool>.Failure(RecipeErrors.Unauthorized);
        }
        var deleteResult = await _recipeService.DeleteRecipeAsync(authenticatedUserId.Value, id);

        return deleteResult;
    }

    //Ingredient endpoints
    [HttpGet("{recipeId:int}/ingredients")]
    [Authorize]
    public async Task<Result<IEnumerable<RecipeIngredientSummaryDto>>> GetIngredients(int recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<IEnumerable<RecipeIngredientSummaryDto>>.Failure(RecipeErrors.Unauthorized);
        }
        var ingredients = await _recipeService.GetIngredientsByRecipeIdAsync(authenticatedUserId.Value, recipeId);
        return ingredients;
    }

    [HttpPost("{recipeId:int}/ingredients")]
    [Authorize]
    public async Task<Result<RecipeIngredientSummaryDto>> AddIngredient(RecipeIngredientCreateDto ingredient)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized);
        }
        var addedIngredient = await _recipeService.AddIngredientToRecipeAsync(authenticatedUserId.Value, ingredient);
        return addedIngredient;
    }
    //Instruction endpoints
    [HttpGet("{recipeId:int}/instructions")]
    [Authorize]
    public async Task<Result<IEnumerable<RecipeInstructionDto>>> GetInstructions(int recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<IEnumerable<RecipeInstructionDto>>.Failure(RecipeErrors.Unauthorized);
        }
        var instructions = await _recipeService.GetInstructionsByRecipeIdAsync(authenticatedUserId.Value, recipeId);
        return instructions;
    }

    [HttpPost("{recipeId:int}/instructions")]
    [Authorize]
    public async Task<Result<RecipeInstructionDto>> AddInstruction(RecipeInstructionCreateDto instruction)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized);
        }
        var addedInstruction = await _recipeService.AddInstructionToRecipeAsync(authenticatedUserId.Value, instruction);
        return addedInstruction;
    }
    //Share endpoints
    // [HttpPost("{recipeId:int}/share")]
    // [Authorize]
    // public async Task<Result<RecipeShareDto>> ShareRecipe(RecipeShareCreateDto share)
    // {
    //     var authenticatedUserId = GetAuthenticatedUserId();
    //     if (authenticatedUserId == null)
    //     {
    //         return Result<RecipeShareDto>.Failure(RecipeErrors.Unauthorized);
    //     }
    //     var shareResult = await _recipeService.CreateShareAsync(authenticatedUserId.Value, share);
    //     return shareResult;
    // }
    // [HttpGet("{recipeId:int}/sharedwith")]
    // [Authorize]
    // public async Task<Result<IEnumerable<RecipeShareDto>>> GetSharedRecipe(int recipeId)
    // {
    //     var authenticatedUserId = GetAuthenticatedUserId();
    //     if (authenticatedUserId == null)
    //     {
    //         return Result<IEnumerable<RecipeShareDto>>.Failure(RecipeErrors.Unauthorized);
    //     }
    //     var shareResult = await _recipeService.GetShareByRecipeIdAsync(authenticatedUserId.Value, recipeId);
    //     return shareResult;
    // }
    // [HttpDelete("{recipeId:int}/unshare")]
    // [Authorize]
    // public async Task<Result<bool>> UnshareRecipe(int recipeId)
    // {
    //     var authenticatedUserId = GetAuthenticatedUserId();
    //     if (authenticatedUserId == null)
    //     {
    //         return Result<bool>.Failure(RecipeErrors.Unauthorized);
    //     }
    //     var unshareResult = await _recipeService.DeleteShareAsync(authenticatedUserId.Value, recipeId);
    //     return unshareResult;
    // }
    // [HttpDelete("share/{shareId:int}")]
    // [Authorize]
    // public async Task<Result<bool>> DeleteShare(int shareId)
    // {
    //     var authenticatedUserId = GetAuthenticatedUserId();
    //     if (authenticatedUserId == null)
    //     {
    //         return Result<bool>.Failure(RecipeErrors.Unauthorized);
    //     }
    //     var deleteResult = await _recipeService.DeleteShareAsync(authenticatedUserId.Value, shareId);
    //     return deleteResult;
    // }
}