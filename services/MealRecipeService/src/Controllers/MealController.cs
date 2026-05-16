using MealRecipeService.Interfaces;
using MealRecipeService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace MealRecipeService.Controllers;

[ApiController]
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
    [Authorize]
    public async Task<IActionResult> GetMeal(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.GetMealByIdAsync(userId.Value, id));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateMeal(MealCreateDto meal)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.CreateMealAsync(userId.Value, meal));
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateMeal(MealUpdateDto meal)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.UpdateMealAsync(userId.Value, meal));
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteMeal(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<bool>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.DeleteMealAsync(userId.Value, id));
    }

    [HttpPost("{mealId:int}/clone")]
    [Authorize]
    public async Task<IActionResult> CloneMeal(int mealId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.CloneMealAsync(userId.Value, mealId));
    }

    [HttpPost("{mealId:int}/share")]
    [Authorize]
    public async Task<IActionResult> ShareMeal(int mealId, [FromBody] ShareRequestDto request)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<ResourcePermissionDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.ShareMealAsync(userId.Value, mealId, request));
    }

    // MealItem endpoints

    [HttpGet("{mealId:int}/recipes")]
    [Authorize]
    public async Task<IActionResult> GetRecipes(int mealId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<IEnumerable<RecipeDto>>.Failure(MealErrors.Unauthorized));

        var mealItemResult = await _mealService.GetMealItemByMealIdAsync(userId.Value, mealId);
        if (!mealItemResult.IsSuccess || mealItemResult.Value == null)
            return HandleResult(Result<IEnumerable<RecipeDto>>.Failure(MealItemErrors.NotFoundMeal));

        var recipeDtos = new List<RecipeDto>();
        foreach (var mealItem in mealItemResult.Value)
        {
            var recipeResult = await _recipeService.GetRecipeByIdAsync(userId.Value, mealItem.RecipeId!.Value);
            if (recipeResult.IsSuccess && recipeResult.Value != null)
                recipeDtos.Add(recipeResult.Value);
        }
        return HandleResult(Result<IEnumerable<RecipeDto>>.Success(recipeDtos));
    }

    [HttpPost("{mealId:int}/items")]
    [Authorize]
    public async Task<IActionResult> AddMealItem(int mealId, [FromBody] MealItemCreateDto mealItem)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealItemDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.AddMealItemAsync(userId.Value, mealItem with { MealId = mealId }));
    }

    [HttpPut("items")]
    [Authorize]
    public async Task<IActionResult> UpdateMealItem([FromBody] MealItemUpdateDto mealItem)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealItemDto>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.UpdateMealItemAsync(userId.Value, mealItem));
    }

    [HttpDelete("items/{mealItemId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteMealItem(int mealItemId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<bool>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.DeleteMealItemAsync(userId.Value, mealItemId));
    }
}
