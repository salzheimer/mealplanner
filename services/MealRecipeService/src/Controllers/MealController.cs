using MealRecipeService.Contracts;
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

    [HttpGet]
    public async Task<IActionResult> GetAllMeals()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<IEnumerable<MealSummaryResponse>>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.GetAllMealsAsync(userId.Value));
    }

    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetMeal(Guid id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDetailResponse>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.GetMealByIdAsync(userId.Value, id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeal([FromBody] CreateMealRequest meal)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDetailResponse>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.CreateMealAsync(userId.Value, meal));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMeal([FromBody] UpdateMealRequest meal)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDetailResponse>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.UpdateMealAsync(userId.Value, meal));
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteMeal(Guid id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<bool>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.DeleteMealAsync(userId.Value, id));
    }

    [HttpPost("{mealId:Guid}/clone")]
    public async Task<IActionResult> CloneMeal(Guid mealId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealDetailResponse>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.CloneMealAsync(userId.Value, mealId));
    }

    [HttpPost("{mealId:Guid}/share")]
    public async Task<IActionResult> ShareMeal(int mealId, [FromBody] ShareMealRequest request)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<ShareRecipeResponse>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.ShareMealAsync(request));
    }
    [HttpGet("shared-with-me")]
    public async Task<IActionResult> MealsSharedWithMe()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<ShareMealResponse>.Failure(MealErrors.Unauthorized));

        return HandleResult(await _mealService.GetMealsSharedWithMeAsync(userId.Value));
    }
    // MealItem endpoints

    /// <summary>
    /// Get recipes associated with a meal
    /// </summary>
    /// <param name="mealId"></param>
    /// <returns>List of recipe details</returns>
    [HttpGet("{mealId:Guid}/recipes")]
    public async Task<IActionResult> GetRecipes(Guid mealId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<IEnumerable<MealDetailResponse>>.Failure(MealErrors.Unauthorized));

        var mealItemResult = await _mealService.GetMealItemsByMealIdAsync(userId.Value, mealId);
        if (!mealItemResult.IsSuccess || mealItemResult.Value == null)
            return HandleResult(Result<IEnumerable<MealDetailResponse>>.Failure(MealItemErrors.NotFoundMeal));

        var recipeDtos = new List<RecipeDetailResponse>();
        foreach (var mealItem in mealItemResult.Value)
        {
            if (mealItem.ItemTypeName != "recipe" || mealItem.RecipeId == null)
                continue;
            var recipeResult = await _recipeService.GetRecipeDetailAsync(userId.Value, mealItem.RecipeId.Value);
            if (recipeResult.IsSuccess && recipeResult.Value != null)
                recipeDtos.Add(recipeResult.Value);
        }
        return HandleResult(Result<IEnumerable<RecipeDetailResponse>>.Success(recipeDtos));
    }

    [HttpPost("{mealId:Guid}/items")]
    public async Task<IActionResult> AddMealItem(Guid mealId, [FromBody] CreateMealItemRequest mealItem)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealItemResponse>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.AddMealItemAsync(userId.Value, mealItem with { MealId = mealId }));
    }

    [HttpPut("items")]
    public async Task<IActionResult> UpdateMealItem([FromBody] UpdateMealItemRequest mealItem)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<MealItemResponse>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.UpdateMealItemAsync(userId.Value, mealItem));
    }

    [HttpDelete("items/{mealItemId:Guid}")]
    public async Task<IActionResult> DeleteMealItem(Guid mealItemId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<bool>.Failure(MealErrors.Unauthorized));
        return HandleResult(await _mealService.DeleteMealItemAsync(userId.Value, mealItemId));
    }
}
