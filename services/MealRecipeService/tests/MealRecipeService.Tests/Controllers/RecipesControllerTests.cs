using MealRecipeService.Controllers;
using MealRecipeService.Interfaces;
using Moq;
using Shared.Models;
using Xunit;

namespace MealRecipeService.Tests.Controllers;

public class RecipesControllerTests
{
    private readonly Mock<IRecipeService> _recipeService;
    private readonly RecipesController _controller;

    public RecipesControllerTests()
    {
        _recipeService = new Mock<IRecipeService>();
        _controller = new RecipesController(_recipeService.Object);
    }

    private static RecipeSummaryDto MakeSummary(int id = 1) => new(
        id, "Pasta Carbonara", "A classic Italian dish", 4, null,
        TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(10), 4, 1, Visibility.Private
    );

    private static RecipeDto MakeRecipe(int id = 1) => new(
        id, "Pasta Carbonara", "A classic Italian dish", "Rich and creamy", 4, null,
        TimeSpan.FromMinutes(20), TimeSpan.FromMinutes(10), 4, 1, Visibility.Private,
        null, null
    );

    private static RecipeIngredientDto MakeIngredient(int id = 1, int recipeId = 1) => new(
        id, recipeId, "Bacon", 200m, "grams"
    );

    private static RecipeInstructionDto MakeInstruction(int id = 1, int recipeId = 1) => new(
        id, recipeId, 1, "Boil pasta", null
    );

    private static RecipeShareDto MakeShare(int id = 1, int recipeId = 1) => new(
        id, recipeId, 2, null, 1, Permission.View, DateTime.UtcNow
    );

    // --- GetAll ---

    [Fact]
    public async Task GetAll_Success_ReturnsSuccessWithRecipes()
    {
        var summaries = new List<RecipeSummaryDto> { MakeSummary(1), MakeSummary(2) };
        _recipeService.Setup(s => s.GetAllRecipesAsync())
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryDto>>.Success(summaries));

        var result = await _controller.GetAll();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetAll_ServiceFailure_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetAllRecipesAsync())
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetAll();

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- GetById ---

    [Fact]
    public async Task GetById_ExistingId_ReturnsRecipe()
    {
        _recipeService.Setup(s => s.GetRecipeByIdAsync(1))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(1)));

        var result = await _controller.GetById(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
        Assert.Equal("Pasta Carbonara", result.Value.Name);
    }

    [Fact]
    public async Task GetById_NonExistentId_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetRecipeByIdAsync(999))
            .ReturnsAsync(Result<RecipeDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetById(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- Create ---

    [Fact]
    public async Task Create_ValidRecipe_ReturnsSuccessWithSummary()
    {
        var createDto = new RecipeCreateDto("Pasta Carbonara", null, null, null, null, null, null, null, 1, Visibility.Private);
        _recipeService.Setup(s => s.CreateRecipeAsync(createDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Success(MakeSummary(1)));

        var result = await _controller.Create(createDto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Pasta Carbonara", result.Value!.Name);
    }

    [Fact]
    public async Task Create_ServiceFailure_ReturnsFailure()
    {
        var createDto = new RecipeCreateDto("", null, null, null, null, null, null, null, 1, null);
        _recipeService.Setup(s => s.CreateRecipeAsync(createDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToCreate));

        var result = await _controller.Create(createDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- Update ---

    [Fact]
    public async Task Update_ExistingRecipe_ReturnsSuccessWithSummary()
    {
        var updateDto = new RecipeUpdateDto(1, "Updated Pasta", null, null, null, null, null, null, null, null);
        _recipeService.Setup(s => s.UpdateRecipeAsync(updateDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Success(MakeSummary(1)));

        var result = await _controller.Update(1, updateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task Update_NonExistentRecipe_ReturnsFailure()
    {
        var updateDto = new RecipeUpdateDto(999, "Updated Pasta", null, null, null, null, null, null, null, null);
        _recipeService.Setup(s => s.UpdateRecipeAsync(updateDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToUpdate));

        var result = await _controller.Update(999, updateDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.UnableToUpdate.Code, result.Error.Code);
    }

    // --- Delete ---

    [Fact]
    public async Task Delete_ExistingRecipe_ReturnsSuccess()
    {
        _recipeService.Setup(s => s.DeleteRecipeAsync(1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.Delete(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task Delete_NonExistentRecipe_ReturnsFailure()
    {
        _recipeService.Setup(s => s.DeleteRecipeAsync(999))
            .ReturnsAsync(Result<bool>.Failure(RecipeErrors.UnableToDelete));

        var result = await _controller.Delete(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.UnableToDelete.Code, result.Error.Code);
    }

    // --- GetIngredients ---

    [Fact]
    public async Task GetIngredients_ValidRecipeId_ReturnsIngredients()
    {
        var ingredients = new List<RecipeIngredientDto> { MakeIngredient(1, 1), MakeIngredient(2, 1) };
        _recipeService.Setup(s => s.GetIngredientsByRecipeIdAsync(1))
            .ReturnsAsync(Result<IEnumerable<RecipeIngredientDto>>.Success(ingredients));

        var result = await _controller.GetIngredients(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetIngredients_NonExistentRecipe_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetIngredientsByRecipeIdAsync(999))
            .ReturnsAsync(Result<IEnumerable<RecipeIngredientDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetIngredients(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- AddIngredient ---

    [Fact]
    public async Task AddIngredient_ValidIngredient_ReturnsSuccessWithIngredient()
    {
        var ingredientDto = MakeIngredient(0, 1);
        _recipeService.Setup(s => s.AddIngredientToRecipeAsync(ingredientDto))
            .ReturnsAsync(Result<RecipeIngredientDto>.Success(MakeIngredient(1, 1)));

        var result = await _controller.AddIngredient(ingredientDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task AddIngredient_ServiceFailure_ReturnsFailure()
    {
        var ingredientDto = MakeIngredient(0, 999);
        _recipeService.Setup(s => s.AddIngredientToRecipeAsync(ingredientDto))
            .ReturnsAsync(Result<RecipeIngredientDto>.Failure(RecipeIngredientErrors.UnableToCreate));

        var result = await _controller.AddIngredient(ingredientDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeIngredientErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- GetInstructions ---

    [Fact]
    public async Task GetInstructions_ValidRecipeId_ReturnsInstructions()
    {
        var instructions = new List<RecipeInstructionDto> { MakeInstruction(1, 1), MakeInstruction(2, 1) };
        _recipeService.Setup(s => s.GetInstructionsByRecipeIdAsync(1))
            .ReturnsAsync(Result<IEnumerable<RecipeInstructionDto>>.Success(instructions));

        var result = await _controller.GetInstructions(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetInstructions_NonExistentRecipe_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetInstructionsByRecipeIdAsync(999))
            .ReturnsAsync(Result<IEnumerable<RecipeInstructionDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetInstructions(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeErrors.NotFound.Code, result.Error.Code);
    }

    // --- AddInstruction ---

    [Fact]
    public async Task AddInstruction_ValidInstruction_ReturnsSuccessWithInstruction()
    {
        var instructionDto = MakeInstruction(0, 1);
        _recipeService.Setup(s => s.AddInstructionToRecipeAsync(instructionDto))
            .ReturnsAsync(Result<RecipeInstructionDto>.Success(MakeInstruction(1, 1)));

        var result = await _controller.AddInstruction(instructionDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task AddInstruction_ServiceFailure_ReturnsFailure()
    {
        var instructionDto = MakeInstruction(0, 999);
        _recipeService.Setup(s => s.AddInstructionToRecipeAsync(instructionDto))
            .ReturnsAsync(Result<RecipeInstructionDto>.Failure(RecipeInstructionErrors.UnableToCreate));

        var result = await _controller.AddInstruction(instructionDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeInstructionErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- ShareRecipe ---

    [Fact]
    public async Task ShareRecipe_ValidShare_ReturnsSuccessWithShare()
    {
        var shareCreateDto = new RecipeShareCreateDto(1, 2, null, 1, Permission.View, null);
        _recipeService.Setup(s => s.CreateShareAsync(shareCreateDto))
            .ReturnsAsync(Result<RecipeShareDto>.Success(MakeShare(1, 1)));

        var result = await _controller.ShareRecipe(shareCreateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task ShareRecipe_ServiceFailure_ReturnsFailure()
    {
        var shareCreateDto = new RecipeShareCreateDto(999, 2, null, 1, Permission.View, null);
        _recipeService.Setup(s => s.CreateShareAsync(shareCreateDto))
            .ReturnsAsync(Result<RecipeShareDto>.Failure(RecipeShareErrors.UnableToCreate));

        var result = await _controller.ShareRecipe(shareCreateDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeShareErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- GetSharedRecipe ---

    [Fact]
    public async Task GetSharedRecipe_ValidRecipeId_ReturnsShares()
    {
        var shares = new List<RecipeShareDto> { MakeShare(1, 1) };
        _recipeService.Setup(s => s.GetShareByRecipeIdAsync(1))
            .ReturnsAsync(Result<IEnumerable<RecipeShareDto>>.Success(shares));

        var result = await _controller.GetSharedRecipe(1);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task GetSharedRecipe_NonExistentRecipe_ReturnsFailure()
    {
        _recipeService.Setup(s => s.GetShareByRecipeIdAsync(999))
            .ReturnsAsync(Result<IEnumerable<RecipeShareDto>>.Failure(RecipeShareErrors.NotFound));

        var result = await _controller.GetSharedRecipe(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeShareErrors.NotFound.Code, result.Error.Code);
    }

    // --- UnshareRecipe ---

    [Fact]
    public async Task UnshareRecipe_ExistingShare_ReturnsSuccess()
    {
        _recipeService.Setup(s => s.DeleteShareAsync(1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.UnshareRecipe(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task UnshareRecipe_NonExistentShare_ReturnsFailure()
    {
        _recipeService.Setup(s => s.DeleteShareAsync(999))
            .ReturnsAsync(Result<bool>.Failure(RecipeShareErrors.UnableToDelete));

        var result = await _controller.UnshareRecipe(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(RecipeShareErrors.UnableToDelete.Code, result.Error.Code);
    }
}
