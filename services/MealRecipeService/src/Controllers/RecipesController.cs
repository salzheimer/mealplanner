using MealRecipeService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MealRecipeService.Controllers;

[ApiController]
[Authorize]
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
    public async Task<IActionResult> GetFullRecipe(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<RecipeDetailDto>.Failure(RecipeErrors.Unauthorized));
        }

        var recipe = HandleResult(await _recipeService.GetRecipeDetailAsync(userId.Value, id));

        return recipe;
    }
    [HttpGet("shared-with-me")]
    public async Task<IActionResult> GetRecipesSharedWithMe()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<RecipeSummaryDto>>.Failure(RecipeErrors.Unauthorized));
        }

        var sharedRecipes = HandleResult(await _recipeService.GetRecipesSharedWithMeAsync(userId.Value));

        return sharedRecipes;
    }

    [HttpPost]

    public async Task<IActionResult> Create([FromBody] RecipeCreateDto recipe)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }

        return HandleResult(await _recipeService.CreateRecipeAsync(authenticatedUserId.Value, recipe));

    }
    [HttpPost("{recipeId:int}/clone")]

    public async Task<IActionResult> Clone(int recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }

        return HandleResult(await _recipeService.CloneRecipeAsync(authenticatedUserId.Value, recipeId));
    }
    [HttpPut("{recipeId:int}")]
    public async Task<IActionResult> Update(int recipeId, [FromBody] RecipeUpdateDto recipe)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }

        return HandleResult(await _recipeService.UpdateRecipeAsync(authenticatedUserId.Value, recipeId, recipe));
    }
    [HttpDelete("{id:int}")]

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

    [HttpPost("{recipeId:int}/ingredients")]

    public async Task<IActionResult> AddIngredient(int recipeId, [FromBody] RecipeIngredientCreateDto ingredient)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }
        var addedIngredient = await _recipeService.AddIngredientToRecipeAsync(authenticatedUserId.Value, recipeId, ingredient);
        return HandleResult(addedIngredient);
    }
    [HttpPut("{recipeId:int}/ingredients/{ingredientId:int}")]

    public async Task<IActionResult> UpdateIngredient(int recipeId, int ingredientId, [FromBody] RecipeIngredientUpdateDto ingredient)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeIngredientSummaryDto>.Failure(RecipeErrors.Unauthorized));
        }
        var updatedIngredient = await _recipeService.UpdateRecipeIngredientAsync(authenticatedUserId.Value, recipeId, ingredientId, ingredient);
        return HandleResult(updatedIngredient);
    }
    [HttpDelete("{recipeId:int}/ingredients/{ingredientId:int}")]

    public async Task<IActionResult> DeleteIngredient(int recipeId, int ingredientId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<bool>.Failure(RecipeErrors.Unauthorized));
        }
        var deleteResult = await _recipeService.DeleteRecipeIngredientAsync(authenticatedUserId.Value, recipeId, ingredientId);
        return HandleResult(deleteResult);
    }
    //Instruction endpoints
    [HttpGet("{recipeId:int}/instructions")]

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

    [HttpPost("{recipeId:int}/instructions")]

    public async Task<IActionResult> AddInstruction(int recipeId, [FromBody] RecipeInstructionCreateDto instruction)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized));
        }
        var addedInstruction = await _recipeService.AddInstructionToRecipeAsync(authenticatedUserId.Value, recipeId, instruction);
        return HandleResult(addedInstruction);
    }
    [HttpPut("{recipeId:int}/instructions/{instructionId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateInstruction(int recipeId, int instructionId, [FromBody] RecipeInstructionUpdateDto instruction)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeInstructionDto>.Failure(RecipeErrors.Unauthorized));
        }
        var updatedInstruction = await _recipeService.UpdateRecipeInstructionAsync(authenticatedUserId.Value, recipeId, instructionId, instruction);
        return HandleResult(updatedInstruction);
    }
    [HttpDelete("{recipeId:int}/instructions/{instructionId:int}")]

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