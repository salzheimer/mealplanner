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

public class RecipesControllerTests
{
    private readonly Mock<IRecipeService> _recipeService;
    private readonly RecipesController _controller;
    private static readonly Guid UserId       = new("10000000-0000-0000-0000-000000000001");
    private static readonly Guid RecipeId     = new("20000000-0000-0000-0000-000000000001");
    private static readonly Guid IngredientId = new("30000000-0000-0000-0000-000000000001");
    private static readonly Guid InstructionId = new("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SubjectId    = new("50000000-0000-0000-0000-000000000001");

    public RecipesControllerTests()
    {
        _recipeService = new Mock<IRecipeService>();
        _controller = new RecipesController(_recipeService.Object);
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

    private static RecipeSummaryResponse MakeSummary(Guid? id = null) => new(
        id ?? RecipeId, "Pasta Carbonara", "A classic Italian dish", null,
        4, null, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(10), 4, UserId
    );

    private static RecipeDetailResponse MakeDetail(Guid? id = null) => new(
        id ?? RecipeId, "Pasta Carbonara", "A classic Italian dish", null,
        4, null, TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(10), 4, UserId, null, null
    );

    private static RecipeIngredientSummaryResponse MakeIngredient(Guid? id = null) => new(
        id ?? IngredientId, RecipeId, "Bacon", 200m, "grams", null
    );

    private static RecipeInstructionResponse MakeInstruction(Guid? id = null) => new(
        id ?? InstructionId, RecipeId, 1, "Boil pasta", null
    );

    private static ShareRecipeResponse MakeShareResponse() => new(
        RecipeId, "recipe", 1, "user", 2, "view", 3, SubjectId, UserId, null
    );

    // --- GetAll ---

    [Fact]
    public async Task GetAll_Success_Returns200WithRecipes()
    {
        var summaries = new List<RecipeSummaryResponse> { MakeSummary(), MakeSummary(Guid.NewGuid()) };
        _recipeService.Setup(s => s.GetAllRecipesAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryResponse>>.Success(summaries));

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeSummaryResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetAll_ServiceFailure_Returns404()
    {
        _recipeService.Setup(s => s.GetAllRecipesAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryResponse>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetAll();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetFullRecipe ---

    [Fact]
    public async Task GetFullRecipe_ExistingId_Returns200WithRecipe()
    {
        _recipeService.Setup(s => s.GetRecipeDetailAsync(UserId, RecipeId))
            .ReturnsAsync(Result<RecipeDetailResponse>.Success(MakeDetail()));

        var result = await _controller.GetFullRecipe(RecipeId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeDetailResponse>(ok.Value);
        Assert.Equal(RecipeId, value.Id);
        Assert.Equal("Pasta Carbonara", value.Name);
    }

    [Fact]
    public async Task GetFullRecipe_NonExistentId_Returns404()
    {
        var unknownId = Guid.NewGuid();
        _recipeService.Setup(s => s.GetRecipeDetailAsync(UserId, unknownId))
            .ReturnsAsync(Result<RecipeDetailResponse>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetFullRecipe(unknownId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetRecipesSharedWithMe ---

    [Fact]
    public async Task GetRecipesSharedWithMe_HasSharedRecipes_Returns200WithList()
    {
        var shared = new List<RecipeSummaryResponse> { MakeSummary() };
        _recipeService.Setup(s => s.GetRecipesSharedWithMeAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryResponse>>.Success(shared));

        var result = await _controller.GetRecipesSharedWithMe();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeSummaryResponse>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetRecipesSharedWithMe_NoneShared_Returns404()
    {
        _recipeService.Setup(s => s.GetRecipesSharedWithMeAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryResponse>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetRecipesSharedWithMe();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- Create ---

    [Fact]
    public async Task Create_ValidRecipe_Returns200WithSummary()
    {
        var request = new CreateRecipeRequest("Pasta Carbonara", null, null, null, null, null, null, null, UserId);
        _recipeService.Setup(s => s.CreateRecipeAsync(UserId, request))
            .ReturnsAsync(Result<RecipeSummaryResponse>.Success(MakeSummary()));

        var result = await _controller.Create(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeSummaryResponse>(ok.Value);
        Assert.Equal("Pasta Carbonara", value.Name);
    }

    [Fact]
    public async Task Create_ServiceFailure_Returns500()
    {
        var request = new CreateRecipeRequest("", null, null, null, null, null, null, null, UserId);
        _recipeService.Setup(s => s.CreateRecipeAsync(UserId, request))
            .ReturnsAsync(Result<RecipeSummaryResponse>.Failure(RecipeErrors.UnableToCreate));

        var result = await _controller.Create(request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- Clone ---

    [Fact]
    public async Task Clone_ValidRecipeId_Returns200WithClonedSummary()
    {
        var cloneId = Guid.NewGuid();
        _recipeService.Setup(s => s.CloneRecipeAsync(UserId, RecipeId))
            .ReturnsAsync(Result<RecipeSummaryResponse>.Success(MakeSummary(cloneId)));

        var result = await _controller.Clone(RecipeId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeSummaryResponse>(ok.Value);
        Assert.Equal(cloneId, value.Id);
    }

    [Fact]
    public async Task Clone_RecipeNotFound_Returns404()
    {
        var unknownId = Guid.NewGuid();
        _recipeService.Setup(s => s.CloneRecipeAsync(UserId, unknownId))
            .ReturnsAsync(Result<RecipeSummaryResponse>.Failure(RecipeErrors.NotFound));

        var result = await _controller.Clone(unknownId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- Update ---

    [Fact]
    public async Task Update_ExistingRecipe_Returns200WithSummary()
    {
        var request = new UpdateRecipeRequest(RecipeId, "Updated Pasta", null, null, null, null, null, null, null, UserId);
        _recipeService.Setup(s => s.UpdateRecipeAsync(UserId, RecipeId, request))
            .ReturnsAsync(Result<RecipeSummaryResponse>.Success(MakeSummary()));

        var result = await _controller.Update(RecipeId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeSummaryResponse>(ok.Value);
        Assert.Equal(RecipeId, value.Id);
    }

    [Fact]
    public async Task Update_NonExistentRecipe_Returns500()
    {
        var unknownId = Guid.NewGuid();
        var request = new UpdateRecipeRequest(unknownId, "Updated Pasta", null, null, null, null, null, null, null, UserId);
        _recipeService.Setup(s => s.UpdateRecipeAsync(UserId, unknownId, request))
            .ReturnsAsync(Result<RecipeSummaryResponse>.Failure(RecipeErrors.UnableToUpdate));

        var result = await _controller.Update(unknownId, request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- Delete ---

    [Fact]
    public async Task Delete_ExistingRecipe_Returns200()
    {
        _recipeService.Setup(s => s.DeleteRecipeAsync(UserId, RecipeId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.Delete(RecipeId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task Delete_NonExistentRecipe_Returns500()
    {
        var unknownId = Guid.NewGuid();
        _recipeService.Setup(s => s.DeleteRecipeAsync(UserId, unknownId))
            .ReturnsAsync(Result<bool>.Failure(RecipeErrors.UnableToDelete));

        var result = await _controller.Delete(unknownId);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- GetIngredients ---

    [Fact]
    public async Task GetIngredients_ValidRecipeId_Returns200WithIngredients()
    {
        var ingredients = new List<RecipeIngredientSummaryResponse> { MakeIngredient(), MakeIngredient(Guid.NewGuid()) };
        _recipeService.Setup(s => s.GetIngredientsByRecipeIdAsync(UserId, RecipeId))
            .ReturnsAsync(Result<IEnumerable<RecipeIngredientSummaryResponse>>.Success(ingredients));

        var result = await _controller.GetIngredients(RecipeId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeIngredientSummaryResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetIngredients_NonExistentRecipe_Returns404()
    {
        var unknownId = Guid.NewGuid();
        _recipeService.Setup(s => s.GetIngredientsByRecipeIdAsync(UserId, unknownId))
            .ReturnsAsync(Result<IEnumerable<RecipeIngredientSummaryResponse>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetIngredients(unknownId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- AddIngredient ---

    [Fact]
    public async Task AddIngredient_ValidIngredient_Returns200WithIngredient()
    {
        var request = new CreateRecipeIngredientRequest("Bacon", 200m, "grams", null);
        _recipeService.Setup(s => s.AddIngredientToRecipeAsync(UserId, RecipeId, request))
            .ReturnsAsync(Result<RecipeIngredientSummaryResponse>.Success(MakeIngredient()));

        var result = await _controller.AddIngredient(RecipeId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeIngredientSummaryResponse>(ok.Value);
        Assert.Equal(IngredientId, value.Id);
    }

    [Fact]
    public async Task AddIngredient_ServiceFailure_Returns500()
    {
        var request = new CreateRecipeIngredientRequest("Bacon", 200m, "grams", null);
        _recipeService.Setup(s => s.AddIngredientToRecipeAsync(UserId, RecipeId, request))
            .ReturnsAsync(Result<RecipeIngredientSummaryResponse>.Failure(RecipeIngredientErrors.UnableToCreate));

        var result = await _controller.AddIngredient(RecipeId, request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- UpdateIngredient ---

    [Fact]
    public async Task UpdateIngredient_ValidIngredient_Returns200WithIngredient()
    {
        var request = new UpdateRecipeIngredientRequest(IngredientId, "Bacon", 200m, "grams", null);
        _recipeService.Setup(s => s.UpdateRecipeIngredientAsync(UserId, RecipeId, IngredientId, request))
            .ReturnsAsync(Result<RecipeIngredientSummaryResponse>.Success(MakeIngredient()));

        var result = await _controller.UpdateIngredient(RecipeId, IngredientId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeIngredientSummaryResponse>(ok.Value);
        Assert.Equal(IngredientId, value.Id);
    }

    [Fact]
    public async Task UpdateIngredient_ServiceFailure_Returns500()
    {
        var unknownId = Guid.NewGuid();
        var request = new UpdateRecipeIngredientRequest(unknownId, "Bacon", 200m, "grams", null);
        _recipeService.Setup(s => s.UpdateRecipeIngredientAsync(UserId, RecipeId, unknownId, request))
            .ReturnsAsync(Result<RecipeIngredientSummaryResponse>.Failure(RecipeIngredientErrors.UnableToUpdate));

        var result = await _controller.UpdateIngredient(RecipeId, unknownId, request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- DeleteIngredient ---

    [Fact]
    public async Task DeleteIngredient_ValidIds_Returns200()
    {
        _recipeService.Setup(s => s.DeleteRecipeIngredientAsync(UserId, RecipeId, IngredientId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteIngredient(RecipeId, IngredientId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteIngredient_Unauthorized_Returns401()
    {
        _recipeService.Setup(s => s.DeleteRecipeIngredientAsync(UserId, RecipeId, IngredientId))
            .ReturnsAsync(Result<bool>.Failure(RecipeErrors.Unauthorized));

        var result = await _controller.DeleteIngredient(RecipeId, IngredientId);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    // --- GetInstructions ---

    [Fact]
    public async Task GetInstructions_ValidRecipeId_Returns200WithInstructions()
    {
        var instructions = new List<RecipeInstructionResponse> { MakeInstruction(), MakeInstruction(Guid.NewGuid()) };
        _recipeService.Setup(s => s.GetInstructionsByRecipeIdAsync(UserId, RecipeId))
            .ReturnsAsync(Result<IEnumerable<RecipeInstructionResponse>>.Success(instructions));

        var result = await _controller.GetInstructions(RecipeId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeInstructionResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetInstructions_NonExistentRecipe_Returns404()
    {
        var unknownId = Guid.NewGuid();
        _recipeService.Setup(s => s.GetInstructionsByRecipeIdAsync(UserId, unknownId))
            .ReturnsAsync(Result<IEnumerable<RecipeInstructionResponse>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetInstructions(unknownId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- AddInstruction ---

    [Fact]
    public async Task AddInstruction_ValidInstruction_Returns200WithInstruction()
    {
        var request = new CreateRecipeInstructionRequest(RecipeId, 1, "Boil pasta", null);
        _recipeService.Setup(s => s.AddInstructionToRecipeAsync(UserId, RecipeId, request))
            .ReturnsAsync(Result<RecipeInstructionResponse>.Success(MakeInstruction()));

        var result = await _controller.AddInstruction(RecipeId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeInstructionResponse>(ok.Value);
        Assert.Equal(InstructionId, value.Id);
    }

    [Fact]
    public async Task AddInstruction_ServiceFailure_Returns500()
    {
        var request = new CreateRecipeInstructionRequest(RecipeId, 1, "Boil pasta", null);
        _recipeService.Setup(s => s.AddInstructionToRecipeAsync(UserId, RecipeId, request))
            .ReturnsAsync(Result<RecipeInstructionResponse>.Failure(RecipeInstructionErrors.UnableToCreate));

        var result = await _controller.AddInstruction(RecipeId, request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- UpdateInstruction ---

    [Fact]
    public async Task UpdateInstruction_ValidInstruction_Returns200WithInstruction()
    {
        var request = new UpdateRecipeInstructionRequest(InstructionId, 1, "Boil pasta", null);
        _recipeService.Setup(s => s.UpdateRecipeInstructionAsync(UserId, RecipeId, InstructionId, request))
            .ReturnsAsync(Result<RecipeInstructionResponse>.Success(MakeInstruction()));

        var result = await _controller.UpdateInstruction(RecipeId, InstructionId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeInstructionResponse>(ok.Value);
        Assert.Equal(InstructionId, value.Id);
    }

    [Fact]
    public async Task UpdateInstruction_ServiceFailure_Returns500()
    {
        var unknownId = Guid.NewGuid();
        var request = new UpdateRecipeInstructionRequest(unknownId, 1, "Boil pasta", null);
        _recipeService.Setup(s => s.UpdateRecipeInstructionAsync(UserId, RecipeId, unknownId, request))
            .ReturnsAsync(Result<RecipeInstructionResponse>.Failure(RecipeInstructionErrors.UnableToUpdate));

        var result = await _controller.UpdateInstruction(RecipeId, unknownId, request);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- DeleteInstruction ---

    [Fact]
    public async Task DeleteInstruction_ValidIds_Returns200()
    {
        _recipeService.Setup(s => s.DeleteRecipeInstructionAsync(UserId, RecipeId, InstructionId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteInstruction(RecipeId, InstructionId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteInstruction_Unauthorized_Returns401()
    {
        _recipeService.Setup(s => s.DeleteRecipeInstructionAsync(UserId, RecipeId, InstructionId))
            .ReturnsAsync(Result<bool>.Failure(RecipeErrors.Unauthorized));

        var result = await _controller.DeleteInstruction(RecipeId, InstructionId);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    // --- ShareRecipe ---

    [Fact]
    public async Task ShareRecipe_ValidRequest_Returns200WithResponse()
    {
        var request = new ShareRecipeRequest(RecipeId, "user", SubjectId, "view", UserId, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(request))
            .ReturnsAsync(Result<ShareRecipeResponse>.Success(MakeShareResponse()));

        var result = await _controller.ShareRecipe(RecipeId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ShareRecipeResponse>(ok.Value);
        Assert.Equal("recipe", value.ResourceTypeName);
        Assert.Equal(SubjectId, value.SubjectId);
    }

    [Fact]
    public async Task ShareRecipe_NotOwner_Returns401()
    {
        var request = new ShareRecipeRequest(RecipeId, "user", SubjectId, "view", UserId, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(request))
            .ReturnsAsync(Result<ShareRecipeResponse>.Failure(RecipeErrors.Unauthorized));

        var result = await _controller.ShareRecipe(RecipeId, request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task ShareRecipe_RecipeNotFound_Returns404()
    {
        var request = new ShareRecipeRequest(RecipeId, "user", SubjectId, "view", UserId, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(request))
            .ReturnsAsync(Result<ShareRecipeResponse>.Failure(RecipeErrors.NotFound));

        var result = await _controller.ShareRecipe(RecipeId, request);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
