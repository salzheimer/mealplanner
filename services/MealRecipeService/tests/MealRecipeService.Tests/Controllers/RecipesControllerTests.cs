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
    private const int UserId = 1;

    public RecipesControllerTests()
    {
        _recipeService = new Mock<IRecipeService>();
        _controller = new RecipesController(_recipeService.Object);
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

    private static RecipeSummaryDto MakeSummary(int id = 1) => new(
        id, "Pasta Carbonara", "A classic Italian dish", 4, null,
        TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(10), 4, 1
    );

    private static RecipeDto MakeRecipe(int id = 1) => new(
        id, "Pasta Carbonara", "A classic Italian dish", "Rich and creamy", 4, null,
        TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(10), 4, 1,
        null, null
    );

    private static RecipeIngredientSummaryDto MakeIngredientSummary(int id = 1, int recipeId = 1) => new(
        id, recipeId, "Bacon", 200m, "grams", null
    );

    private static RecipeIngredientCreateDto MakeIngredientCreate(int recipeId = 1) => new(
        recipeId, "Bacon", 200m, "grams", null
    );

    private static RecipeInstructionDto MakeInstruction(int id = 1, int recipeId = 1) => new(
        id, recipeId, 1, "Boil pasta", null
    );

    private static RecipeInstructionCreateDto MakeInstructionCreate(int recipeId = 1) => new(
        recipeId, 1, "Boil pasta", null
    );

    private static ResourcePermissionDto MakePermission(int recipeId = 1) => new(
        1, ResourceType.Recipe, recipeId, SubjectType.User, 2, Permission.View, UserId
    );

    // --- GetAll ---

    [Fact]
    public async Task GetAll_Success_ReturnsSuccessWithRecipes()
    {
        var summaries = new List<RecipeSummaryDto> { MakeSummary(1), MakeSummary(2) };
        _recipeService.Setup(s => s.GetAllRecipesAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryDto>>.Success(summaries));

        var result = await _controller.GetAll();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetAll_ServiceFailure_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetAllRecipesAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetAll();

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- GetFullRecipe ---

    [Fact]
    public async Task GetFullRecipe_ExistingId_ReturnsRecipe()
    {
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 1))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(1)));

        var result = await _controller.GetFullRecipe(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
        Assert.Equal("Pasta Carbonara", result.Value.Name);
    }

    [Fact]
    public async Task GetFullRecipe_NonExistentId_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 999))
            .ReturnsAsync(Result<RecipeDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetFullRecipe(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- Create ---

    [Fact]
    public async Task Create_ValidRecipe_ReturnsSuccessWithSummary()
    {
        var createDto = new RecipeCreateDto("Pasta Carbonara", null, null, null, null, null, null, null, 1);
        _recipeService.Setup(s => s.CreateRecipeAsync(UserId, createDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Success(MakeSummary(1)));

        var result = await _controller.Create(createDto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Pasta Carbonara", result.Value!.Name);
    }

    [Fact]
    public async Task Create_ServiceFailure_ReturnsFailure()
    {
        var createDto = new RecipeCreateDto("", null, null, null, null, null, null, null, 1);
        _recipeService.Setup(s => s.CreateRecipeAsync(UserId, createDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToCreate));

        var result = await _controller.Create(createDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- Clone ---

    [Fact]
    public async Task Clone_ValidRecipeId_ReturnsClonedSummary()
    {
        _recipeService.Setup(s => s.CloneRecipeAsync(UserId, 1))
            .ReturnsAsync(Result<RecipeSummaryDto>.Success(MakeSummary(2)));

        var result = await _controller.Clone(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Id);
    }

    [Fact]
    public async Task Clone_RecipeNotFound_ReturnsFailure()
    {
        _recipeService.Setup(s => s.CloneRecipeAsync(UserId, 999))
            .ReturnsAsync(Result<RecipeSummaryDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.Clone(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- Update ---

    [Fact]
    public async Task Update_ExistingRecipe_ReturnsSuccessWithSummary()
    {
        var updateDto = new RecipeUpdateDto(1, "Updated Pasta", null, null, null, null, null, null, null);
        _recipeService.Setup(s => s.UpdateRecipeAsync(UserId, updateDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Success(MakeSummary(1)));

        var result = await _controller.Update(updateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task Update_NonExistentRecipe_ReturnsFailure()
    {
        var updateDto = new RecipeUpdateDto(999, "Updated Pasta", null, null, null, null, null, null, null);
        _recipeService.Setup(s => s.UpdateRecipeAsync(UserId, updateDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToUpdate));

        var result = await _controller.Update(updateDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.UnableToUpdate.Code, result.Error.Code);
    }

    // --- Delete ---

    [Fact]
    public async Task Delete_ExistingRecipe_ReturnsSuccess()
    {
        _recipeService.Setup(s => s.DeleteRecipeAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.Delete(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Delete_NonExistentRecipe_ReturnsFailure()
    {
        _recipeService.Setup(s => s.DeleteRecipeAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(RecipeErrors.UnableToDelete));

        var result = await _controller.Delete(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.UnableToDelete.Code, result.Error.Code);
    }

    // --- GetIngredients ---

    [Fact]
    public async Task GetIngredients_ValidRecipeId_ReturnsIngredients()
    {
        var ingredients = new List<RecipeIngredientSummaryDto> { MakeIngredientSummary(1, 1), MakeIngredientSummary(2, 1) };
        _recipeService.Setup(s => s.GetIngredientsByRecipeIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<RecipeIngredientSummaryDto>>.Success(ingredients));

        var result = await _controller.GetIngredients(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetIngredients_NonExistentRecipe_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetIngredientsByRecipeIdAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<RecipeIngredientSummaryDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetIngredients(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- AddIngredient ---

    [Fact]
    public async Task AddIngredient_ValidIngredient_ReturnsSuccessWithIngredient()
    {
        var ingredientDto = MakeIngredientCreate(1);
        _recipeService.Setup(s => s.AddIngredientToRecipeAsync(UserId, ingredientDto))
            .ReturnsAsync(Result<RecipeIngredientSummaryDto>.Success(MakeIngredientSummary(1, 1)));

        var result = await _controller.AddIngredient(ingredientDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task AddIngredient_ServiceFailure_ReturnsFailure()
    {
        var ingredientDto = MakeIngredientCreate(999);
        _recipeService.Setup(s => s.AddIngredientToRecipeAsync(UserId, ingredientDto))
            .ReturnsAsync(Result<RecipeIngredientSummaryDto>.Failure(RecipeIngredientErrors.UnableToCreate));

        var result = await _controller.AddIngredient(ingredientDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeIngredientErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- GetInstructions ---

    [Fact]
    public async Task GetInstructions_ValidRecipeId_ReturnsInstructions()
    {
        var instructions = new List<RecipeInstructionDto> { MakeInstruction(1, 1), MakeInstruction(2, 1) };
        _recipeService.Setup(s => s.GetInstructionsByRecipeIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<RecipeInstructionDto>>.Success(instructions));

        var result = await _controller.GetInstructions(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetInstructions_NonExistentRecipe_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetInstructionsByRecipeIdAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<RecipeInstructionDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetInstructions(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- AddInstruction ---

    [Fact]
    public async Task AddInstruction_ValidInstruction_ReturnsSuccessWithInstruction()
    {
        var instructionDto = MakeInstructionCreate(1);
        _recipeService.Setup(s => s.AddInstructionToRecipeAsync(UserId, instructionDto))
            .ReturnsAsync(Result<RecipeInstructionDto>.Success(MakeInstruction(1, 1)));

        var result = await _controller.AddInstruction(instructionDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task AddInstruction_ServiceFailure_ReturnsFailure()
    {
        var instructionDto = MakeInstructionCreate(999);
        _recipeService.Setup(s => s.AddInstructionToRecipeAsync(UserId, instructionDto))
            .ReturnsAsync(Result<RecipeInstructionDto>.Failure(RecipeInstructionErrors.UnableToCreate));

        var result = await _controller.AddInstruction(instructionDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeInstructionErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- ShareRecipe ---

    [Fact]
    public async Task ShareRecipe_ValidRequest_ReturnsPermission()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(UserId, 1, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Success(MakePermission(1)));

        var result = await _controller.ShareRecipe(1, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(ResourceType.Recipe, result.Value!.ResourceType);
        Assert.Equal(2, result.Value.SubjectId);
    }

    [Fact]
    public async Task ShareRecipe_NotOwner_ReturnsUnauthorized()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(UserId, 1, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(RecipeErrors.Unauthorized));

        var result = await _controller.ShareRecipe(1, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.Unauthorized.Code, result.Error.Code);
    }

    [Fact]
    public async Task ShareRecipe_RecipeNotFound_ReturnsFailure()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(UserId, 999, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.ShareRecipe(999, request);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }
}
