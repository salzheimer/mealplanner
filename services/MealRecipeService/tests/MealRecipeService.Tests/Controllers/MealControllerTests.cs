using MealRecipeService.Contracts;
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
    private static readonly Guid UserId    = new("10000000-0000-0000-0000-000000000001");
    private static readonly Guid MealId    = new("20000000-0000-0000-0000-000000000001");
    private static readonly Guid RecipeId  = new("30000000-0000-0000-0000-000000000001");
    private static readonly Guid ItemId    = new("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SubjectId = new("50000000-0000-0000-0000-000000000001");

    public MealControllerTests()
    {
        _mealService = new Mock<IMealService>();
        _recipeService = new Mock<IRecipeService>();
        _controller = new MealController(_mealService.Object, _recipeService.Object);
        SetAuthenticatedUser(_controller, UserId);
    }

    private static void SetAuthenticatedUser(ControllerBase controller, Guid userId)
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

    private static MealDetailResponse MakeMeal(Guid? id = null) => new(
        id ?? MealId, "Monday Dinner", "A hearty dinner", null,
        1, false, UserId, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, UserId
    );

    private static MealItemDetailResponse MakeMealItem(Guid? id = null, Guid? mealId = null, Guid? recipeId = null) => new(
        id ?? ItemId, "Pasta Carbonara", recipeId ?? RecipeId, mealId ?? MealId,
        1, "recipe", UserId, UserId, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow
    );

    private static RecipeDetailResponse MakeRecipe(Guid? id = null) => new(
        id ?? RecipeId, "Pasta Carbonara", null, null, null, null, null, null, null, UserId, null, null
    );

    private static ShareMealResponse MakeShareMealResponse() => new(
        MealId, "meal", 1, "user", 2, "view", 1, SubjectId, UserId, null
    );

    // --- GetMeal ---

    [Fact]
    public async Task GetMeal_ExistingId_Returns200WithMeal()
    {
        _mealService.Setup(s => s.GetMealByIdAsync(UserId, MealId))
            .ReturnsAsync(Result<MealDetailResponse>.Success(MakeMeal()));

        var result = await _controller.GetMeal(MealId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealDetailResponse>(ok.Value);
        Assert.Equal(MealId, value.Id);
        Assert.Equal("Monday Dinner", value.Name);
    }

    [Fact]
    public async Task GetMeal_NonExistentId_Returns404()
    {
        var unknownId = Guid.NewGuid();
        _mealService.Setup(s => s.GetMealByIdAsync(UserId, unknownId))
            .ReturnsAsync(Result<MealDetailResponse>.Failure(MealErrors.NotFound));

        var result = await _controller.GetMeal(unknownId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- CreateMeal ---

    [Fact]
    public async Task CreateMeal_ValidMeal_Returns200WithMeal()
    {
        var request = new CreateMealRequest("Monday Dinner", null, null, 1, false, UserId);
        _mealService.Setup(s => s.CreateMealAsync(UserId, request))
            .ReturnsAsync(Result<MealDetailResponse>.Success(MakeMeal()));

        var result = await _controller.CreateMeal(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealDetailResponse>(ok.Value);
        Assert.Equal("Monday Dinner", value.Name);
    }

    [Fact]
    public async Task CreateMeal_ServiceFailure_Returns400()
    {
        var request = new CreateMealRequest("", null, null, 1, false, UserId);
        _mealService.Setup(s => s.CreateMealAsync(UserId, request))
            .ReturnsAsync(Result<MealDetailResponse>.Failure(MealErrors.UnableToCreate));

        var result = await _controller.CreateMeal(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- UpdateMeal ---

    [Fact]
    public async Task UpdateMeal_ExistingMeal_Returns200WithMeal()
    {
        var request = new UpdateMealRequest(MealId, "Updated Dinner", null, null, 1, false, UserId);
        _mealService.Setup(s => s.UpdateMealAsync(UserId, request))
            .ReturnsAsync(Result<MealDetailResponse>.Success(MakeMeal()));

        var result = await _controller.UpdateMeal(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealDetailResponse>(ok.Value);
        Assert.Equal(MealId, value.Id);
    }

    [Fact]
    public async Task UpdateMeal_NonExistentMeal_Returns400()
    {
        var request = new UpdateMealRequest(Guid.NewGuid(), "Updated Dinner", null, null, 1, false, UserId);
        _mealService.Setup(s => s.UpdateMealAsync(UserId, request))
            .ReturnsAsync(Result<MealDetailResponse>.Failure(MealErrors.UnableToUpdate));

        var result = await _controller.UpdateMeal(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- DeleteMeal ---

    [Fact]
    public async Task DeleteMeal_ExistingMeal_Returns200()
    {
        _mealService.Setup(s => s.DeleteMealAsync(UserId, MealId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteMeal(MealId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteMeal_NonExistentMeal_Returns400()
    {
        var unknownId = Guid.NewGuid();
        _mealService.Setup(s => s.DeleteMealAsync(UserId, unknownId))
            .ReturnsAsync(Result<bool>.Failure(MealErrors.UnableToDelete));

        var result = await _controller.DeleteMeal(unknownId);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- CloneMeal ---

    [Fact]
    public async Task CloneMeal_ValidMealId_Returns200WithClonedMeal()
    {
        var cloneId = Guid.NewGuid();
        _mealService.Setup(s => s.CloneMealAsync(UserId, MealId))
            .ReturnsAsync(Result<MealDetailResponse>.Success(MakeMeal(cloneId)));

        var result = await _controller.CloneMeal(MealId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealDetailResponse>(ok.Value);
        Assert.Equal(cloneId, value.Id);
    }

    [Fact]
    public async Task CloneMeal_MealNotFound_Returns404()
    {
        var unknownId = Guid.NewGuid();
        _mealService.Setup(s => s.CloneMealAsync(UserId, unknownId))
            .ReturnsAsync(Result<MealDetailResponse>.Failure(MealErrors.NotFound));

        var result = await _controller.CloneMeal(unknownId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- ShareMeal ---

    [Fact]
    public async Task ShareMeal_ValidRequest_Returns200WithResponse()
    {
        var request = new ShareMealRequest(MealId, "user", SubjectId, "view", UserId, null);
        _mealService.Setup(s => s.ShareMealAsync(request))
            .ReturnsAsync(Result<ShareMealResponse>.Success(MakeShareMealResponse()));

        var result = await _controller.ShareMeal(0, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ShareMealResponse>(ok.Value);
        Assert.Equal("meal", value.ResourceTypeName);
        Assert.Equal(SubjectId, value.SubjectId);
    }

    [Fact]
    public async Task ShareMeal_NotOwner_Returns401()
    {
        var request = new ShareMealRequest(MealId, "user", SubjectId, "view", UserId, null);
        _mealService.Setup(s => s.ShareMealAsync(request))
            .ReturnsAsync(Result<ShareMealResponse>.Failure(MealErrors.Unauthorized));

        var result = await _controller.ShareMeal(0, request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task ShareMeal_MealNotFound_Returns404()
    {
        var request = new ShareMealRequest(MealId, "user", SubjectId, "view", UserId, null);
        _mealService.Setup(s => s.ShareMealAsync(request))
            .ReturnsAsync(Result<ShareMealResponse>.Failure(MealErrors.NotFound));

        var result = await _controller.ShareMeal(0, request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- MealsSharedWithMe ---

    [Fact]
    public async Task MealsSharedWithMe_HasSharedMeals_Returns200WithList()
    {
        var shared = new List<MealSummaryResponse>
        {
            new(MealId, "Monday Dinner", null, null, 1, false, UserId)
        };
        _mealService.Setup(s => s.GetMealsSharedWithMeAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<MealSummaryResponse>>.Success(shared));

        var result = await _controller.MealsSharedWithMe();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<MealSummaryResponse>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task MealsSharedWithMe_NoneShared_Returns404()
    {
        _mealService.Setup(s => s.GetMealsSharedWithMeAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<MealSummaryResponse>>.Failure(MealErrors.NotFound));

        var result = await _controller.MealsSharedWithMe();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetRecipes ---

    [Fact]
    public async Task GetRecipes_ValidMealWithItems_Returns200WithAllRecipes()
    {
        var recipeId2 = Guid.NewGuid();
        var items = new List<MealItemDetailResponse>
        {
            MakeMealItem(recipeId: RecipeId),
            MakeMealItem(recipeId: recipeId2)
        };
        _mealService.Setup(s => s.GetMealItemsByMealIdAsync(UserId, MealId))
            .ReturnsAsync(Result<IEnumerable<MealItemDetailResponse>>.Success(items));
        _recipeService.Setup(s => s.GetRecipeDetailAsync(UserId, RecipeId))
            .ReturnsAsync(Result<RecipeDetailResponse>.Success(MakeRecipe(RecipeId)));
        _recipeService.Setup(s => s.GetRecipeDetailAsync(UserId, recipeId2))
            .ReturnsAsync(Result<RecipeDetailResponse>.Success(MakeRecipe(recipeId2)));

        var result = await _controller.GetRecipes(MealId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeDetailResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetRecipes_EmptyMeal_Returns200WithEmptyList()
    {
        _mealService.Setup(s => s.GetMealItemsByMealIdAsync(UserId, MealId))
            .ReturnsAsync(Result<IEnumerable<MealItemDetailResponse>>.Success(new List<MealItemDetailResponse>()));

        var result = await _controller.GetRecipes(MealId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeDetailResponse>>(ok.Value);
        Assert.Empty(value);
    }

    [Fact]
    public async Task GetRecipes_MealNotFound_Returns404()
    {
        var unknownId = Guid.NewGuid();
        _mealService.Setup(s => s.GetMealItemsByMealIdAsync(UserId, unknownId))
            .ReturnsAsync(Result<IEnumerable<MealItemDetailResponse>>.Failure(MealItemErrors.NotFoundMeal));

        var result = await _controller.GetRecipes(unknownId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetRecipes_OneRecipeNotFound_Returns200WithPartialList()
    {
        var missingRecipeId = Guid.NewGuid();
        var items = new List<MealItemDetailResponse>
        {
            MakeMealItem(recipeId: RecipeId),
            MakeMealItem(recipeId: missingRecipeId)
        };
        _mealService.Setup(s => s.GetMealItemsByMealIdAsync(UserId, MealId))
            .ReturnsAsync(Result<IEnumerable<MealItemDetailResponse>>.Success(items));
        _recipeService.Setup(s => s.GetRecipeDetailAsync(UserId, RecipeId))
            .ReturnsAsync(Result<RecipeDetailResponse>.Success(MakeRecipe(RecipeId)));
        _recipeService.Setup(s => s.GetRecipeDetailAsync(UserId, missingRecipeId))
            .ReturnsAsync(Result<RecipeDetailResponse>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetRecipes(MealId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeDetailResponse>>(ok.Value);
        Assert.Single(value);
    }

    // --- AddMealItem ---

    [Fact]
    public async Task AddMealItem_ValidItem_Returns200WithMealItem()
    {
        var request = new CreateMealItemRequest("Pasta Carbonara", RecipeId, MealId, 1, "recipe", UserId);
        _mealService.Setup(s => s.AddMealItemAsync(UserId, It.IsAny<CreateMealItemRequest>()))
            .ReturnsAsync(Result<MealItemDetailResponse>.Success(MakeMealItem()));

        var result = await _controller.AddMealItem(MealId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealItemDetailResponse>(ok.Value);
        Assert.Equal(ItemId, value.Id);
    }

    [Fact]
    public async Task AddMealItem_ServiceFailure_Returns400()
    {
        var request = new CreateMealItemRequest("Pasta Carbonara", null, MealId, 1, "recipe", UserId);
        _mealService.Setup(s => s.AddMealItemAsync(UserId, It.IsAny<CreateMealItemRequest>()))
            .ReturnsAsync(Result<MealItemDetailResponse>.Failure(MealItemErrors.UnableToCreate));

        var result = await _controller.AddMealItem(MealId, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- UpdateMealItem ---

    [Fact]
    public async Task UpdateMealItem_ExistingItem_Returns200WithMealItem()
    {
        var request = new UpdateMealItemRequest(ItemId, "Updated Pasta", RecipeId, MealId, 1, "recipe", UserId);
        _mealService.Setup(s => s.UpdateMealItemAsync(UserId, request))
            .ReturnsAsync(Result<MealItemDetailResponse>.Success(MakeMealItem()));

        var result = await _controller.UpdateMealItem(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealItemDetailResponse>(ok.Value);
        Assert.Equal(ItemId, value.Id);
    }

    [Fact]
    public async Task UpdateMealItem_NonExistentItem_Returns400()
    {
        var request = new UpdateMealItemRequest(Guid.NewGuid(), "Updated Pasta", RecipeId, MealId, 1, "recipe", UserId);
        _mealService.Setup(s => s.UpdateMealItemAsync(UserId, request))
            .ReturnsAsync(Result<MealItemDetailResponse>.Failure(MealItemErrors.UnableToUpdate));

        var result = await _controller.UpdateMealItem(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- DeleteMealItem ---

    [Fact]
    public async Task DeleteMealItem_ExistingItem_Returns200()
    {
        _mealService.Setup(s => s.DeleteMealItemAsync(UserId, ItemId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteMealItem(ItemId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteMealItem_NonExistentItem_Returns400()
    {
        var unknownId = Guid.NewGuid();
        _mealService.Setup(s => s.DeleteMealItemAsync(UserId, unknownId))
            .ReturnsAsync(Result<bool>.Failure(MealItemErrors.UnableToDelete));

        var result = await _controller.DeleteMealItem(unknownId);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
