using MealRecipeService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MealRecipeService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
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
         
        var recipes = await _recipeService.GetAllRecipesAsync();
        return recipes;
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<Result<RecipeDto>> GetById(int id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);
        
        return recipe;
    }
    [HttpPost]
    [Authorize]
    public async Task<Result<RecipeSummaryDto>> Create(RecipeCreateDto recipe)
    {
        var createdRecipe = await _recipeService.CreateRecipeAsync(recipe);
        if (!createdRecipe.IsSuccess)
        {
            return Result<RecipeSummaryDto>.Failure(createdRecipe.Error);
        }
        return createdRecipe;

    }
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<Result<RecipeSummaryDto>> Update(int id, RecipeUpdateDto recipe)
    {
        var updatedRecipe = await _recipeService.UpdateRecipeAsync(recipe);
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
        var deleteResult = await _recipeService.DeleteRecipeAsync(id);

        return deleteResult;
    }

    //Ingredient endpoints
    [HttpGet("{recipeId:int}/ingredients")]
    [Authorize]
    public async Task<Result<IEnumerable<RecipeIngredientDto>>> GetIngredients(int recipeId)
    {
        var ingredients = await _recipeService.GetIngredientsByRecipeIdAsync(recipeId);
        return ingredients;
    }

    [HttpPost("{recipeId:int}/ingredients")]
    [Authorize]
    public async Task<Result<RecipeIngredientDto>> AddIngredient( RecipeIngredientDto ingredient)
    {
        
        var addedIngredient = await _recipeService.AddIngredientToRecipeAsync( ingredient);
        return addedIngredient;
    }
    //Instruction endpoints
    [HttpGet("{recipeId:int}/instructions")]
    [Authorize]
    public async Task<Result<IEnumerable<RecipeInstructionDto>>> GetInstructions(int recipeId)
    {
        var instructions = await _recipeService.GetInstructionsByRecipeIdAsync(recipeId);
        return instructions;
    }

    [HttpPost("{recipeId:int}/instructions")]
    [Authorize]
    public async Task<Result<RecipeInstructionDto>> AddInstruction( RecipeInstructionDto instruction)
    {
        var addedInstruction = await _recipeService.AddInstructionToRecipeAsync(instruction);
        return addedInstruction;
    }
    //Share endpoints
    [HttpPost("{recipeId:int}/share")]
    [Authorize]
    public async Task<Result<RecipeShareDto>> ShareRecipe(RecipeShareCreateDto share)
    {
        var shareResult = await _recipeService.CreateShareAsync(share);
        return shareResult;
    }
    [HttpGet("{recipeId:int}/share")]
    [Authorize]
    public async Task<Result<IEnumerable<RecipeShareDto>>> GetSharedRecipe(int recipeId)
    {

        var shareResult = await _recipeService.GetShareByRecipeIdAsync(recipeId);
        return shareResult;
    }
    [HttpDelete("{recipeId:int}/share")]
    [Authorize]
    public async Task<Result<bool>> UnshareRecipe(int recipeId)
    {
        var unshareResult = await _recipeService.DeleteShareAsync(recipeId);
        return unshareResult;
    }

}