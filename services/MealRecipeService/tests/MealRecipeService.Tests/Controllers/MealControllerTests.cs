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
    public async Task GetMeal_ExistingId_Returns200WithMeal()
    {
        _mealService.Setup(s => s.GetMealByIdAsync(UserId, 1))
            .ReturnsAsync(Result<MealDto>.Success(MakeMeal(1)));

        var result = await _controller.GetMeal(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealDto>(ok.Value);
        Assert.Equal(1, value.Id);
        Assert.Equal("Monday Dinner", value.Name);
    }

    [Fact]
    public async Task GetMeal_NonExistentId_Returns404()
    {
        _mealService.Setup(s => s.GetMealByIdAsync(UserId, 999))
            .ReturnsAsync(Result<MealDto>.Failure(MealErrors.NotFound));

        var result = await _controller.GetMeal(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- CreateMeal ---

    [Fact]
    public async Task CreateMeal_ValidMeal_Returns200WithMeal()
    {
        var createDto = new MealCreateDto(MealType.Dinner, "Monday Dinner", null, null, null);
        _mealService.Setup(s => s.CreateMealAsync(UserId, createDto))
            .ReturnsAsync(Result<MealDto>.Success(MakeMeal(1)));

        var result = await _controller.CreateMeal(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealDto>(ok.Value);
        Assert.Equal("Monday Dinner", value.Name);
        Assert.Equal(MealType.Dinner, value.MealType);
    }

    [Fact]
    public async Task CreateMeal_ServiceFailure_Returns400()
    {
        var createDto = new MealCreateDto(MealType.Dinner, "", null, null, null);
        _mealService.Setup(s => s.CreateMealAsync(UserId, createDto))
            .ReturnsAsync(Result<MealDto>.Failure(MealErrors.UnableToCreate));

        var result = await _controller.CreateMeal(createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- UpdateMeal ---

    [Fact]
    public async Task UpdateMeal_ExistingMeal_Returns200WithMeal()
    {
        var updateDto = new MealUpdateDto(1, MealType.Dinner, "Updated Dinner", null, null, null, null);
        _mealService.Setup(s => s.UpdateMealAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealDto>.Success(MakeMeal(1)));

        var result = await _controller.UpdateMeal(updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdateMeal_NonExistentMeal_Returns400()
    {
        var updateDto = new MealUpdateDto(999, MealType.Dinner, "Updated Dinner", null, null, null, null);
        _mealService.Setup(s => s.UpdateMealAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealDto>.Failure(MealErrors.UnableToUpdate));

        var result = await _controller.UpdateMeal(updateDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- DeleteMeal ---

    [Fact]
    public async Task DeleteMeal_ExistingMeal_Returns200()
    {
        _mealService.Setup(s => s.DeleteMealAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteMeal(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteMeal_NonExistentMeal_Returns400()
    {
        _mealService.Setup(s => s.DeleteMealAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(MealErrors.UnableToDelete));

        var result = await _controller.DeleteMeal(999);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- CloneMeal ---

    [Fact]
    public async Task CloneMeal_ValidMealId_Returns200WithClonedMeal()
    {
        _mealService.Setup(s => s.CloneMealAsync(UserId, 1))
            .ReturnsAsync(Result<MealDto>.Success(MakeMeal(2)));

        var result = await _controller.CloneMeal(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealDto>(ok.Value);
        Assert.Equal(2, value.Id);
    }

    [Fact]
    public async Task CloneMeal_MealNotFound_Returns404()
    {
        _mealService.Setup(s => s.CloneMealAsync(UserId, 999))
            .ReturnsAsync(Result<MealDto>.Failure(MealErrors.NotFound));

        var result = await _controller.CloneMeal(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- ShareMeal ---

    [Fact]
    public async Task ShareMeal_ValidRequest_Returns200WithPermission()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _mealService.Setup(s => s.ShareMealAsync(UserId, 1, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Success(MakePermission(1)));

        var result = await _controller.ShareMeal(1, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ResourcePermissionDto>(ok.Value);
        Assert.Equal(ResourceType.Meal, value.ResourceType);
        Assert.Equal(2, value.SubjectId);
    }

    [Fact]
    public async Task ShareMeal_NotOwner_Returns401()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _mealService.Setup(s => s.ShareMealAsync(UserId, 1, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(MealErrors.Unauthorized));

        var result = await _controller.ShareMeal(1, request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task ShareMeal_MealNotFound_Returns404()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _mealService.Setup(s => s.ShareMealAsync(UserId, 999, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(MealErrors.NotFound));

        var result = await _controller.ShareMeal(999, request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetRecipes ---

    [Fact]
    public async Task GetRecipes_ValidMealWithItems_Returns200WithAllRecipes()
    {
        var mealItems = new List<MealItemDto> { MakeMealItem(1, 1, 1), MakeMealItem(2, 1, 2) };
        _mealService.Setup(s => s.GetMealItemByMealIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<MealItemDto>>.Success(mealItems));
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 1))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(1)));
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 2))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(2)));

        var result = await _controller.GetRecipes(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetRecipes_EmptyMeal_Returns200WithEmptyList()
    {
        _mealService.Setup(s => s.GetMealItemByMealIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<MealItemDto>>.Success(new List<MealItemDto>()));

        var result = await _controller.GetRecipes(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeDto>>(ok.Value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task GetRecipes_MealNotFound_Returns404()
    {
        _mealService.Setup(s => s.GetMealItemByMealIdAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<MealItemDto>>.Failure(MealItemErrors.NotFoundMeal));

        var result = await _controller.GetRecipes(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetRecipes_OneRecipeNotFound_Returns200WithPartialList()
    {
        var mealItems = new List<MealItemDto> { MakeMealItem(1, 1, 1), MakeMealItem(2, 1, 999) };
        _mealService.Setup(s => s.GetMealItemByMealIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<MealItemDto>>.Success(mealItems));
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 1))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(1)));
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 999))
            .ReturnsAsync(Result<RecipeDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetRecipes(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeDto>>(ok.Value);
        Assert.Single(value);
    }

    // --- AddMealItem ---

    [Fact]
    public async Task AddMealItem_ValidItem_Returns200WithMealItem()
    {
        var createDto = new MealItemCreateDto("Pasta Carbonara", 1, 1, ItemType.Recipe);
        var expectedDto = createDto with { MealId = 1 };
        _mealService.Setup(s => s.AddMealItemAsync(UserId, expectedDto))
            .ReturnsAsync(Result<MealItemDto>.Success(MakeMealItem(1, 1, 1)));

        var result = await _controller.AddMealItem(1, createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealItemDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task AddMealItem_ServiceFailure_Returns400()
    {
        var createDto = new MealItemCreateDto("Pasta Carbonara", 1, 999, ItemType.Recipe);
        var expectedDto = createDto with { MealId = 1 };
        _mealService.Setup(s => s.AddMealItemAsync(UserId, expectedDto))
            .ReturnsAsync(Result<MealItemDto>.Failure(MealItemErrors.UnableToCreate));

        var result = await _controller.AddMealItem(1, createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- UpdateMealItem ---

    [Fact]
    public async Task UpdateMealItem_ExistingItem_Returns200WithMealItem()
    {
        var updateDto = new MealItemUpdateDto(1, "Updated Pasta", 1, 1, ItemType.Recipe);
        _mealService.Setup(s => s.UpdateMealItemAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealItemDto>.Success(MakeMealItem(1, 1, 1)));

        var result = await _controller.UpdateMealItem(updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealItemDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdateMealItem_NonExistentItem_Returns400()
    {
        var updateDto = new MealItemUpdateDto(999, "Updated Pasta", 1, 1, ItemType.Recipe);
        _mealService.Setup(s => s.UpdateMealItemAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealItemDto>.Failure(MealItemErrors.UnableToUpdate));

        var result = await _controller.UpdateMealItem(updateDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- DeleteMealItem ---

    [Fact]
    public async Task DeleteMealItem_ExistingItem_Returns200()
    {
        _mealService.Setup(s => s.DeleteMealItemAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteMealItem(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteMealItem_NonExistentItem_Returns400()
    {
        _mealService.Setup(s => s.DeleteMealItemAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(MealItemErrors.UnableToDelete));

        var result = await _controller.DeleteMealItem(999);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
