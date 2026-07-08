using MealRecipeService.Contracts;
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

    //Recipe endpoGuids
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<RecipeSummaryResponse>>.Failure(RecipeErrors.Unauthorized));
        }
        var recipes = HandleResult(await _recipeService.GetAllRecipesAsync(userId.Value));
        return recipes;
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetFullRecipe(Guid id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<RecipeDetailResponse>.Failure(RecipeErrors.Unauthorized));
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
            return HandleResult(Result<IEnumerable<RecipeSummaryResponse>>.Failure(RecipeErrors.Unauthorized));
        }

        var sharedRecipes = HandleResult(await _recipeService.GetRecipesSharedWithMeAsync(userId.Value));

        return sharedRecipes;
    }

    [HttpPost]

    public async Task<IActionResult> Create([FromBody] CreateRecipeRequest recipe)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryResponse>.Failure(RecipeErrors.Unauthorized));
        }

        return HandleResult(await _recipeService.CreateRecipeAsync(authenticatedUserId.Value, recipe));

    }
    [HttpPost("{recipeId:Guid}/clone")]

    public async Task<IActionResult> Clone(Guid recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryResponse>.Failure(RecipeErrors.Unauthorized));
        }

        return HandleResult(await _recipeService.CloneRecipeAsync(authenticatedUserId.Value, recipeId));
    }
    [HttpPut("{recipeId:Guid}")]
    public async Task<IActionResult> Update(Guid recipeId, [FromBody] UpdateRecipeRequest recipe)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeSummaryResponse>.Failure(RecipeErrors.Unauthorized));
        }

        return HandleResult(await _recipeService.UpdateRecipeAsync(authenticatedUserId.Value, recipeId, recipe));
    }
    [HttpDelete("{id:Guid}")]

    public async Task<IActionResult> Delete(Guid id)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<bool>.Failure(RecipeErrors.Unauthorized));
        }
        var deleteResult = await _recipeService.DeleteRecipeAsync(authenticatedUserId.Value, id);

        return HandleResult(deleteResult);
    }

    //Ingredient endpoGuids
    [HttpGet("{recipeId:Guid}/ingredients")]
     
    public async Task<IActionResult> GetIngredients(Guid recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<RecipeIngredientSummaryResponse>>.Failure(RecipeErrors.Unauthorized));
        }
        var ingredients = await _recipeService.GetIngredientsByRecipeIdAsync(authenticatedUserId.Value, recipeId);
        return HandleResult(ingredients);
    }

    [HttpPost("{recipeId:Guid}/ingredients")]

    public async Task<IActionResult> AddIngredient(Guid recipeId, [FromBody] CreateRecipeIngredientRequest ingredient)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeIngredientSummaryResponse>.Failure(RecipeErrors.Unauthorized));
        }
        var addedIngredient = await _recipeService.AddIngredientToRecipeAsync(authenticatedUserId.Value, recipeId, ingredient);
        return HandleResult(addedIngredient);
    }
    [HttpPut("{recipeId:Guid}/ingredients/{ingredientId:Guid}")]

    public async Task<IActionResult> UpdateIngredient(Guid recipeId, Guid ingredientId, [FromBody] UpdateRecipeIngredientRequest ingredient)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeIngredientSummaryResponse>.Failure(RecipeErrors.Unauthorized));
        }
        var updatedIngredient = await _recipeService.UpdateRecipeIngredientAsync(authenticatedUserId.Value, recipeId, ingredientId, ingredient);
        return HandleResult(updatedIngredient);
    }
    [HttpDelete("{recipeId:Guid}/ingredients/{ingredientId:Guid}")]

    public async Task<IActionResult> DeleteIngredient(Guid recipeId, Guid ingredientId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<bool>.Failure(RecipeErrors.Unauthorized));
        }
        var deleteResult = await _recipeService.DeleteRecipeIngredientAsync(authenticatedUserId.Value, recipeId, ingredientId);
        return HandleResult(deleteResult);
    }
    //Instruction endpoGuids
    [HttpGet("{recipeId:Guid}/instructions")]

    public async Task<IActionResult> GetInstructions(Guid recipeId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<RecipeInstructionResponse>>.Failure(RecipeErrors.Unauthorized));
        }
        var instructions = await _recipeService.GetInstructionsByRecipeIdAsync(authenticatedUserId.Value, recipeId);
        return HandleResult(instructions);
    }

    [HttpPost("{recipeId:Guid}/instructions")]

    public async Task<IActionResult> AddInstruction(Guid recipeId, [FromBody] CreateRecipeInstructionRequest instruction)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeInstructionResponse>.Failure(RecipeErrors.Unauthorized));
        }
        var addedInstruction = await _recipeService.AddInstructionToRecipeAsync(authenticatedUserId.Value, recipeId, instruction);
        return HandleResult(addedInstruction);
    }
    [HttpPut("{recipeId:Guid}/instructions/{instructionId:Guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateInstruction(Guid recipeId, Guid instructionId, [FromBody] UpdateRecipeInstructionRequest instruction)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<RecipeInstructionResponse>.Failure(RecipeErrors.Unauthorized));
        }
        var updatedInstruction = await _recipeService.UpdateRecipeInstructionAsync(authenticatedUserId.Value, recipeId, instructionId, instruction);
        return HandleResult(updatedInstruction);
    }
    [HttpDelete("{recipeId:Guid}/instructions/{instructionId:Guid}")]

    public async Task<IActionResult> DeleteInstruction(Guid recipeId, Guid instructionId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<bool>.Failure(RecipeErrors.Unauthorized));
        }
        var deleteResult = await _recipeService.DeleteRecipeInstructionAsync(authenticatedUserId.Value, recipeId, instructionId);
        return HandleResult(deleteResult);
    }

    //Share endpoGuids
    [HttpPost("{recipeId:Guid}/share")]

    public async Task<IActionResult> ShareRecipe(Guid recipeId, [FromBody] ShareRecipeRequest request)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<ShareRecipeResponse>.Failure(RecipeErrors.Unauthorized));
        }
        return HandleResult(await _recipeService.ShareRecipeAsync(request));
    }
}