using MealRecipeService.Interfaces;
using MealRecipeService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MealRecipeService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class MealController : ControllerBase
{
    private readonly IMealService _mealService;
    private readonly IRecipeService _recipeService;
    public MealController(IMealService mealService, IRecipeService recipeService)
    {
        _mealService = mealService;
        _recipeService = recipeService;
    }
    // Meal endpoints
    [HttpGet]
    [Authorize]
    public async Task<Result<MealDto>> GetMeal(int id)
    {
        var meal = await _mealService.GetMealByIdAsync(id);
        return meal;
    }

    [HttpPost]
    [Authorize]
    public async Task<Result<MealDto>> CreateMeal(MealCreateDto meal)
    {
        var createdMeal = await _mealService.CreateMealAsync(meal);

        return createdMeal;
    }
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<Result<MealDto>> UpdateMeal(MealUpdateDto meal)
    {
        var updatedMeal = await _mealService.UpdateMealAsync(meal);
        return updatedMeal;
    }
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<Result<bool>> DeleteMeal(int id)
    {
        var deleteResult = await _mealService.DeleteMealAsync(id);
        return deleteResult;
    }
    [HttpGet("{mealId:int}/recipes")]
    [Authorize]
    public async Task<Result<IEnumerable<RecipeDto>>> GetRecipes(int mealId)
    {
        var mealItemResult = await _mealService.GetMealItemByMealIdAsync(mealId);
        if (mealItemResult.IsSuccess && mealItemResult.Value != null)
        {
            var recipeDtos = new List<RecipeDto>();
            foreach (var mealItem in mealItemResult.Value)
            {
                var recipeResult = await _recipeService.GetRecipeByIdAsync(mealItem.RecipeId!.Value);
                if (recipeResult.IsSuccess && recipeResult.Value != null)
                {
                    recipeDtos.Add(recipeResult.Value);
                }
            }
            return Result<IEnumerable<RecipeDto>>.Success(recipeDtos);
        }
        else
        {
            return Result<IEnumerable<RecipeDto>>.Failure(MealItemErrors.NotFoundMeal);
        }
        

    }
    }