using MealRecipeService.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MealRecipeService.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MealController : BaseController
{
    private readonly IMealService _mealService;
    private readonly IRecipeService _recipeService;

    public MealController(IMealService mealService, IRecipeService recipeService)
    {
        _mealService = mealService;
        _recipeService = recipeService;
    }

    // Meal endpoints

    [HttpGet("{id:int}")]
       public async Task<IActionResult> GetMeal(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.GetMealByIdAsync(userId.Value, id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeal([FromBody] MealCreateDto meal)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.CreateMealAsync(userId.Value, meal));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMeal([FromBody] MealUpdateDto meal)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.UpdateMealAsync(userId.Value, meal));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMeal(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<bool>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.DeleteMealAsync(userId.Value, id));
    }

    [HttpPost("{mealId:int}/clone")]
    public async Task<IActionResult> CloneMeal(int mealId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.CloneMealAsync(userId.Value, mealId));
    }

    [HttpPost("{mealId:int}/share")]
    public async Task<IActionResult> ShareMeal(int mealId, [FromBody] ShareRequestDto request)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<ResourcePermissionDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.ShareMealAsync(userId.Value, mealId, request));
    }
    [HttpGet("shared-with-me")]
    public async Task<IActionResult> MealsSharedWithMe()
    {
        var userId = GetAuthenticatedUserId();
         if (userId == null)
            return HandleResult(Result<ResourcePermissionDto>.Failure(MealErrors.Unauthorized));

            return HandleResult(await _mealService.GetMealsSharedWithMeAsync(userId.Value));
    }
    // MealItem endpoints

    [HttpGet("{mealId:int}/recipes")]
    public async Task<IActionResult> GetRecipes(int mealId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<IEnumerable<RecipeSummaryDto>>.Failure(MealErrors.Unauthorized));

        var mealItemResult = await _mealService.GetMealItemByMealIdAsync(userId.Value, mealId);
        if (!mealItemResult.IsSuccess || mealItemResult.Value == null)
            return HandleResult(Result<IEnumerable<RecipeDetailDto>>.Failure(MealItemErrors.NotFoundMeal));

        var recipeDtos = new List<RecipeDetailDto>();
        foreach (var mealItem in mealItemResult.Value)
        {   if(mealItem.ItemType != Shared.Models.ItemType.Recipe || mealItem.RecipeId == null)
                continue;
            var recipeResult = await _recipeService.GetRecipeDetailAsync(userId.Value, mealItem.RecipeId.Value);
            if (recipeResult.IsSuccess && recipeResult.Value != null)
                recipeDtos.Add(recipeResult.Value);
        }
        return HandleResult(Result<IEnumerable<RecipeDetailDto>>.Success(recipeDtos));
    }

    [HttpPost("{mealId:int}/items")]
    public async Task<IActionResult> AddMealItem(int mealId, [FromBody] MealItemCreateDto mealItem)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealItemDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.AddMealItemAsync(userId.Value, mealItem with { MealId = mealId }));
    }

    [HttpPut("items")]
    public async Task<IActionResult> UpdateMealItem([FromBody] MealItemUpdateDto mealItem)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealItemDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.UpdateMealItemAsync(userId.Value, mealItem));
    }

    [HttpDelete("items/{mealItemId:int}")]
    public async Task<IActionResult> DeleteMealItem(int mealItemId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<bool>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.DeleteMealItemAsync(userId.Value, mealItemId));
    }
}
