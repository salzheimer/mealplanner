using MealRecipeService.Controllers;
using MealRecipeService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Models;
using System.Security.Claims;
using Xunit;

namespace MealRecipeService.Tests.Controllers;

public class MealControllerTests
{
    private readonly Mock<IMealService> _mealService;
    private readonly Mock<IRecipeService> _recipeService;
    private readonly MealController _controller;
    private const int UserId = 1;

    public MealControllerTests()
    {
        _mealService = new Mock<IMealService>();
        _recipeService = new Mock<IRecipeService>();
        _controller = new MealController(_mealService.Object, _recipeService.Object);
        SetAuthenticatedUser(_controller, UserId);
    }

    private static void SetAuthenticatedUser(ControllerBase controller, int userId)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
            }
        };
    }

    private static MealDto MakeMeal(int id = 1) => new(
        id, "Monday Dinner", "A hearty dinner", null, MealType.Dinner, false,
        DateTime.UtcNow, DateTime.UtcNow
    );

    private static MealItemDto MakeMealItem(int id = 1, int mealId = 1, int recipeId = 1) => new(
        id, "Pasta Carbonara", mealId, recipeId, ItemType.Recipe
    );

    private static RecipeDto MakeRecipe(int id = 1) => new(
        id, "Pasta Carbonara", null, null, null, null, null, null, null, 1,
        null, null
    );

    private static ResourcePermissionDto MakePermission(int mealId = 1) => new(
        1, ResourceType.Meal, mealId, SubjectType.User, 2, Permission.View, UserId
    );

    // --- GetMeal ---

    [Fact]
    public async Task GetMeal_ExistingId_ReturnsMeal()
    {
        _mealService.Setup(s => s.GetMealByIdAsync(1))
            .ReturnsAsync(Result<MealDto>.Success(MakeMeal(1)));

        var result = await _controller.GetMeal(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
        Assert.Equal("Monday Dinner", result.Value.Name);
    }

    [Fact]
    public async Task GetMeal_NonExistentId_ReturnsFailure()
    {
        _mealService.Setup(s => s.GetMealByIdAsync(999))
            .ReturnsAsync(Result<MealDto>.Failure(MealErrors.NotFound));

        var result = await _controller.GetMeal(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealErrors.NotFound.Code, result.Error.Code);
    }

    // --- CreateMeal ---

    [Fact]
    public async Task CreateMeal_ValidMeal_ReturnsSuccessWithMeal()
    {
        var createDto = new MealCreateDto(MealType.Dinner, "Monday Dinner", null, null, null);
        _mealService.Setup(s => s.CreateMealAsync(UserId, createDto))
            .ReturnsAsync(Result<MealDto>.Success(MakeMeal(1)));

        var result = await _controller.CreateMeal(createDto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Monday Dinner", result.Value!.Name);
        Assert.Equal(MealType.Dinner, result.Value.MealType);
    }

    [Fact]
    public async Task CreateMeal_ServiceFailure_ReturnsFailure()
    {
        var createDto = new MealCreateDto(MealType.Dinner, "", null, null, null);
        _mealService.Setup(s => s.CreateMealAsync(UserId, createDto))
            .ReturnsAsync(Result<MealDto>.Failure(MealErrors.UnableToCreate));

        var result = await _controller.CreateMeal(createDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- UpdateMeal ---

    [Fact]
    public async Task UpdateMeal_ExistingMeal_ReturnsSuccessWithMeal()
    {
        var updateDto = new MealUpdateDto(1, MealType.Dinner, "Updated Dinner", null, null, null, null);
        _mealService.Setup(s => s.UpdateMealAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealDto>.Success(MakeMeal(1)));

        var result = await _controller.UpdateMeal(updateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task UpdateMeal_NonExistentMeal_ReturnsFailure()
    {
        var updateDto = new MealUpdateDto(999, MealType.Dinner, "Updated Dinner", null, null, null, null);
        _mealService.Setup(s => s.UpdateMealAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealDto>.Failure(MealErrors.UnableToUpdate));

        var result = await _controller.UpdateMeal(updateDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealErrors.UnableToUpdate.Code, result.Error.Code);
    }

    // --- DeleteMeal ---

    [Fact]
    public async Task DeleteMeal_ExistingMeal_ReturnsSuccess()
    {
        _mealService.Setup(s => s.DeleteMealAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteMeal(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task DeleteMeal_NonExistentMeal_ReturnsFailure()
    {
        _mealService.Setup(s => s.DeleteMealAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(MealErrors.UnableToDelete));

        var result = await _controller.DeleteMeal(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealErrors.UnableToDelete.Code, result.Error.Code);
    }

    // --- ShareMeal ---

    [Fact]
    public async Task ShareMeal_ValidRequest_ReturnsPermission()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _mealService.Setup(s => s.ShareMealAsync(UserId, 1, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Success(MakePermission(1)));

        var result = await _controller.ShareMeal(1, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(ResourceType.Meal, result.Value!.ResourceType);
        Assert.Equal(2, result.Value.SubjectId);
    }

    [Fact]
    public async Task ShareMeal_NotOwner_ReturnsUnauthorized()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _mealService.Setup(s => s.ShareMealAsync(UserId, 1, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(MealErrors.Unauthorized));

        var result = await _controller.ShareMeal(1, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealErrors.Unauthorized.Code, result.Error.Code);
    }

    [Fact]
    public async Task ShareMeal_MealNotFound_ReturnsFailure()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _mealService.Setup(s => s.ShareMealAsync(UserId, 999, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(MealErrors.NotFound));

        var result = await _controller.ShareMeal(999, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealErrors.NotFound.Code, result.Error.Code);
    }

    // --- GetRecipes ---

    [Fact]
    public async Task GetRecipes_ValidMealWithItems_ReturnsAllRecipes()
    {
        var mealItems = new List<MealItemDto> { MakeMealItem(1, 1, 1), MakeMealItem(2, 1, 2) };
        _mealService.Setup(s => s.GetMealItemByMealIdAsync(1))
            .ReturnsAsync(Result<IEnumerable<MealItemDto>>.Success(mealItems));
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 1))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(1)));
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 2))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(2)));

        var result = await _controller.GetRecipes(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetRecipes_EmptyMeal_ReturnsEmptyList()
    {
        _mealService.Setup(s => s.GetMealItemByMealIdAsync(1))
            .ReturnsAsync(Result<IEnumerable<MealItemDto>>.Success(new List<MealItemDto>()));

        var result = await _controller.GetRecipes(1);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task GetRecipes_MealNotFound_ReturnsFailure()
    {
        _mealService.Setup(s => s.GetMealItemByMealIdAsync(999))
            .ReturnsAsync(Result<IEnumerable<MealItemDto>>.Failure(MealItemErrors.NotFoundMeal));

        var result = await _controller.GetRecipes(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealItemErrors.NotFoundMeal.Code, result.Error.Code);
    }

    [Fact]
    public async Task GetRecipes_OneRecipeNotFound_SkipsAndReturnsPartialList()
    {
        var mealItems = new List<MealItemDto> { MakeMealItem(1, 1, 1), MakeMealItem(2, 1, 999) };
        _mealService.Setup(s => s.GetMealItemByMealIdAsync(1))
            .ReturnsAsync(Result<IEnumerable<MealItemDto>>.Success(mealItems));
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 1))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(1)));
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 999))
            .ReturnsAsync(Result<RecipeDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetRecipes(1);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }
}
