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
    public async Task<IActionResult> GetAll()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<RecipeSummaryDto>>.Failure(RecipeErrors.Unauthorized));
        }
        var recipes = HandleResult(await _recipeService.GetAllRecipesAsync(userId.Value));
        return recipes;
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetFullRecipe(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<RecipeDto>.Failure(RecipeErrors.Unauthorized));
        }

        var recipe = HandleResult(await _recipeService.GetRecipeByIdAsync(userId.Value, id));

        return recipe;
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] RecipeCreateDto recipe)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }
        var createdRecipe = await _recipeService.CreateRecipeAsync(authenticatedUserId.Value, recipe);
        if (!createdRecipe.IsSuccess)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(createdRecipe.Error));
        }
        return HandleResult(createdRecipe);

    }
    [HttpPost("{recipeId:int}/clone")]
    [Authorize]
    public async Task<IActionResult> Clone(int recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }
        var clonedRecipe = await _recipeService.CloneRecipeAsync(authenticatedUserId.Value, recipeId);
        if (!clonedRecipe.IsSuccess)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(clonedRecipe.Error));
        }
        return HandleResult(clonedRecipe);
    }
    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] RecipeUpdateDto recipe)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }
        var updatedRecipe = await _recipeService.UpdateRecipeAsync(authenticatedUserId.Value, recipe);
        if (!updatedRecipe.IsSuccess)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(updatedRecipe.Error));
        }
        return HandleResult(updatedRecipe);
    }
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<bool>.Failure(RecipeErrors.Unauthorized));
        }
        var deleteResult = await _recipeService.DeleteRecipeAsync(authenticatedUserId.Value, id);

        return HandleResult(deleteResult);
    }

    //Ingredient endpoints
    [HttpGet("{recipeId:int}/ingredients")]
    [Authorize]
    public async Task<IActionResult> GetIngredients(int recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<RecipeIngredientSummaryDto>>.Failure(RecipeErrors.Unauthorized));
        }
        var ingredients = await _recipeService.GetIngredientsByRecipeIdAsync(authenticatedUserId.Value, recipeId);
        return HandleResult(ingredients);
    }

    [HttpPost("{recipeId:int}/add-ingredient")]
    [Authorize]
    public async Task<IActionResult> AddIngredient([FromBody] RecipeIngredientCreateDto ingredient)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }
        var addedIngredient = await _recipeService.AddIngredientToRecipeAsync(authenticatedUserId.Value, ingredient);
        return HandleResult(addedIngredient);
    }
    [HttpPut("{recipeId:int}/update-ingredient")]
    [Authorize]
    public async Task<IActionResult> UpdateIngredient([FromBody] RecipeIngredientSummaryDto ingredient)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)        {
            return HandleResult(Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }
        var updatedIngredient = await _recipeService.UpdateRecipeIngredientAsync(authenticatedUserId.Value, ingredient);
        return HandleResult(updatedIngredient);
    }
    [HttpDelete("{recipeId:int}/ingredients/{ingredientId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteIngredient(int recipeId, int ingredientId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)        {
            return HandleResult(Result<bool>.Failure(RecipeErrors.Unauthorized));
        }
        var deleteResult = await _recipeService.DeleteRecipeIngredientAsync(authenticatedUserId.Value, recipeId, ingredientId);
        return HandleResult(deleteResult);
    }
    //Instruction endpoints
    [HttpGet("{recipeId:int}/instructions")]
    [Authorize]
    public async Task<IActionResult> GetInstructions(int recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<RecipeInstructionDto>>.Failure(RecipeErrors.Unauthorized));
        }
        var instructions = await _recipeService.GetInstructionsByRecipeIdAsync(authenticatedUserId.Value, recipeId);
        return HandleResult(instructions);
    }

    [HttpPost("{recipeId:int}/add-instruction")]
    [Authorize]
    public async Task<IActionResult> AddInstruction([FromBody] RecipeInstructionCreateDto instruction)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized))    ;
        }
        var addedInstruction = await _recipeService.AddInstructionToRecipeAsync(authenticatedUserId.Value, instruction);
        return HandleResult(addedInstruction);
    }
    [HttpPut("{recipeId:int}/update-instruction")]
    [Authorize]
    public async Task<IActionResult> UpdateInstruction([FromBody] RecipeInstructionDto instruction)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)        {
            return HandleResult(Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized));
        }
        var updatedInstruction = await _recipeService.UpdateRecipeInstructionAsync(authenticatedUserId.Value, instruction);
        return HandleResult(updatedInstruction);
    }
    [HttpDelete("{recipeId:int}/instructions/{instructionId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteInstruction(int recipeId, int instructionId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<bool>.Failure(RecipeErrors.Unauthorized));
        }
        var deleteResult = await _recipeService.DeleteRecipeInstructionAsync(authenticatedUserId.Value, recipeId, instructionId);
        return HandleResult(deleteResult);
    }

    //Share endpoints
    [HttpPost("{recipeId:int}/share")]
    [Authorize]
    public async Task<IActionResult> ShareRecipe(int recipeId, [FromBody] ShareRequestDto request)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
            {
                return HandleResult(Result<ResourcePermissionDto>.Failure(RecipeErrors.Unauthorized));
            }
        return HandleResult(await _recipeService.ShareRecipeAsync(authenticatedUserId.Value, recipeId, request));
    }
}