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
    public async Task GetAll_Success_Returns200WithRecipes()
    {
        var summaries = new List<RecipeSummaryDto> { MakeSummary(1), MakeSummary(2) };
        _recipeService.Setup(s => s.GetAllRecipesAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryDto>>.Success(summaries));

        var result = await _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeSummaryDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetAll_ServiceFailure_Returns404()
    {
        _recipeService.Setup(s => s.GetAllRecipesAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<RecipeSummaryDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetAll();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetFullRecipe ---

    [Fact]
    public async Task GetFullRecipe_ExistingId_Returns200WithRecipe()
    {
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 1))
            .ReturnsAsync(Result<RecipeDto>.Success(MakeRecipe(1)));

        var result = await _controller.GetFullRecipe(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeDto>(ok.Value);
        Assert.Equal(1, value.Id);
        Assert.Equal("Pasta Carbonara", value.Name);
    }

    [Fact]
    public async Task GetFullRecipe_NonExistentId_Returns404()
    {
        _recipeService.Setup(s => s.GetRecipeByIdAsync(UserId, 999))
            .ReturnsAsync(Result<RecipeDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetFullRecipe(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- Create ---

    [Fact]
    public async Task Create_ValidRecipe_Returns200WithSummary()
    {
        var createDto = new RecipeCreateDto("Pasta Carbonara", null, null, null, null, null, null, null, 1);
        _recipeService.Setup(s => s.CreateRecipeAsync(UserId, createDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Success(MakeSummary(1)));

        var result = await _controller.Create(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeSummaryDto>(ok.Value);
        Assert.Equal("Pasta Carbonara", value.Name);
    }

    [Fact]
    public async Task Create_ServiceFailure_Returns500()
    {
        var createDto = new RecipeCreateDto("", null, null, null, null, null, null, null, 1);
        _recipeService.Setup(s => s.CreateRecipeAsync(UserId, createDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToCreate));

        var result = await _controller.Create(createDto);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- Clone ---

    [Fact]
    public async Task Clone_ValidRecipeId_Returns200WithClonedSummary()
    {
        _recipeService.Setup(s => s.CloneRecipeAsync(UserId, 1))
            .ReturnsAsync(Result<RecipeSummaryDto>.Success(MakeSummary(2)));

        var result = await _controller.Clone(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeSummaryDto>(ok.Value);
        Assert.Equal(2, value.Id);
    }

    [Fact]
    public async Task Clone_RecipeNotFound_Returns404()
    {
        _recipeService.Setup(s => s.CloneRecipeAsync(UserId, 999))
            .ReturnsAsync(Result<RecipeSummaryDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.Clone(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- Update ---

    [Fact]
    public async Task Update_ExistingRecipe_Returns200WithSummary()
    {
        var updateDto = new RecipeUpdateDto(1, "Updated Pasta", null, null, null, null, null, null, null, null);
        _recipeService.Setup(s => s.UpdateRecipeAsync(UserId, updateDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Success(MakeSummary(1)));

        var result = await _controller.Update(updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeSummaryDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task Update_NonExistentRecipe_Returns500()
    {
        var updateDto = new RecipeUpdateDto(999, "Updated Pasta", null, null, null, null, null, null, null, null);
        _recipeService.Setup(s => s.UpdateRecipeAsync(UserId, updateDto))
            .ReturnsAsync(Result<RecipeSummaryDto>.Failure(RecipeErrors.UnableToUpdate));

        var result = await _controller.Update(updateDto);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- Delete ---

    [Fact]
    public async Task Delete_ExistingRecipe_Returns200()
    {
        _recipeService.Setup(s => s.DeleteRecipeAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.Delete(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task Delete_NonExistentRecipe_Returns500()
    {
        _recipeService.Setup(s => s.DeleteRecipeAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(RecipeErrors.UnableToDelete));

        var result = await _controller.Delete(999);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- GetIngredients ---

    [Fact]
    public async Task GetIngredients_ValidRecipeId_Returns200WithIngredients()
    {
        var ingredients = new List<RecipeIngredientSummaryDto> { MakeIngredientSummary(1, 1), MakeIngredientSummary(2, 1) };
        _recipeService.Setup(s => s.GetIngredientsByRecipeIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<RecipeIngredientSummaryDto>>.Success(ingredients));

        var result = await _controller.GetIngredients(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeIngredientSummaryDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetIngredients_NonExistentRecipe_Returns404()
    {
        _recipeService.Setup(s => s.GetIngredientsByRecipeIdAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<RecipeIngredientSummaryDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetIngredients(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- AddIngredient ---

    [Fact]
    public async Task AddIngredient_ValidIngredient_Returns200WithIngredient()
    {
        var ingredientDto = MakeIngredientCreate(1);
        _recipeService.Setup(s => s.AddIngredientToRecipeAsync(UserId, ingredientDto))
            .ReturnsAsync(Result<RecipeIngredientSummaryDto>.Success(MakeIngredientSummary(1, 1)));

        var result = await _controller.AddIngredient(ingredientDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeIngredientSummaryDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task AddIngredient_ServiceFailure_Returns500()
    {
        var ingredientDto = MakeIngredientCreate(999);
        _recipeService.Setup(s => s.AddIngredientToRecipeAsync(UserId, ingredientDto))
            .ReturnsAsync(Result<RecipeIngredientSummaryDto>.Failure(RecipeIngredientErrors.UnableToCreate));

        var result = await _controller.AddIngredient(ingredientDto);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- UpdateIngredient ---

    [Fact]
    public async Task UpdateIngredient_ValidIngredient_Returns200WithIngredient()
    {
        var ingredientDto = MakeIngredientSummary(1, 1);
        _recipeService.Setup(s => s.UpdateRecipeIngredientAsync(UserId, ingredientDto))
            .ReturnsAsync(Result<RecipeIngredientSummaryDto>.Success(MakeIngredientSummary(1, 1)));

        var result = await _controller.UpdateIngredient(ingredientDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeIngredientSummaryDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdateIngredient_ServiceFailure_Returns500()
    {
        var ingredientDto = MakeIngredientSummary(999, 1);
        _recipeService.Setup(s => s.UpdateRecipeIngredientAsync(UserId, ingredientDto))
            .ReturnsAsync(Result<RecipeIngredientSummaryDto>.Failure(RecipeIngredientErrors.UnableToUpdate));

        var result = await _controller.UpdateIngredient(ingredientDto);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- DeleteIngredient ---

    [Fact]
    public async Task DeleteIngredient_ValidIds_Returns200()
    {
        _recipeService.Setup(s => s.DeleteRecipeIngredientAsync(UserId, 1, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteIngredient(1, 1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteIngredient_Unauthorized_Returns401()
    {
        _recipeService.Setup(s => s.DeleteRecipeIngredientAsync(UserId, 1, 99))
            .ReturnsAsync(Result<bool>.Failure(RecipeErrors.Unauthorized));

        var result = await _controller.DeleteIngredient(1, 99);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    // --- GetInstructions ---

    [Fact]
    public async Task GetInstructions_ValidRecipeId_Returns200WithInstructions()
    {
        var instructions = new List<RecipeInstructionDto> { MakeInstruction(1, 1), MakeInstruction(2, 1) };
        _recipeService.Setup(s => s.GetInstructionsByRecipeIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<RecipeInstructionDto>>.Success(instructions));

        var result = await _controller.GetInstructions(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<RecipeInstructionDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetInstructions_NonExistentRecipe_Returns404()
    {
        _recipeService.Setup(s => s.GetInstructionsByRecipeIdAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<RecipeInstructionDto>>.Failure(RecipeErrors.NotFound));

        var result = await _controller.GetInstructions(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- AddInstruction ---

    [Fact]
    public async Task AddInstruction_ValidInstruction_Returns200WithInstruction()
    {
        var instructionDto = MakeInstructionCreate(1);
        _recipeService.Setup(s => s.AddInstructionToRecipeAsync(UserId, instructionDto))
            .ReturnsAsync(Result<RecipeInstructionDto>.Success(MakeInstruction(1, 1)));

        var result = await _controller.AddInstruction(instructionDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeInstructionDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task AddInstruction_ServiceFailure_Returns500()
    {
        var instructionDto = MakeInstructionCreate(999);
        _recipeService.Setup(s => s.AddInstructionToRecipeAsync(UserId, instructionDto))
            .ReturnsAsync(Result<RecipeInstructionDto>.Failure(RecipeInstructionErrors.UnableToCreate));

        var result = await _controller.AddInstruction(instructionDto);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- UpdateInstruction ---

    [Fact]
    public async Task UpdateInstruction_ValidInstruction_Returns200WithInstruction()
    {
        var instructionDto = MakeInstruction(1, 1);
        _recipeService.Setup(s => s.UpdateRecipeInstructionAsync(UserId, instructionDto))
            .ReturnsAsync(Result<RecipeInstructionDto>.Success(MakeInstruction(1, 1)));

        var result = await _controller.UpdateInstruction(instructionDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RecipeInstructionDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdateInstruction_ServiceFailure_Returns500()
    {
        var instructionDto = MakeInstruction(999, 1);
        _recipeService.Setup(s => s.UpdateRecipeInstructionAsync(UserId, instructionDto))
            .ReturnsAsync(Result<RecipeInstructionDto>.Failure(RecipeInstructionErrors.UnableToUpdate));

        var result = await _controller.UpdateInstruction(instructionDto);

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    // --- DeleteInstruction ---

    [Fact]
    public async Task DeleteInstruction_ValidIds_Returns200()
    {
        _recipeService.Setup(s => s.DeleteRecipeInstructionAsync(UserId, 1, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteInstruction(1, 1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteInstruction_Unauthorized_Returns401()
    {
        _recipeService.Setup(s => s.DeleteRecipeInstructionAsync(UserId, 1, 99))
            .ReturnsAsync(Result<bool>.Failure(RecipeErrors.Unauthorized));

        var result = await _controller.DeleteInstruction(1, 99);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    // --- ShareRecipe ---

    [Fact]
    public async Task ShareRecipe_ValidRequest_Returns200WithPermission()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(UserId, 1, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Success(MakePermission(1)));

        var result = await _controller.ShareRecipe(1, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<ResourcePermissionDto>(ok.Value);
        Assert.Equal(ResourceType.Recipe, value.ResourceType);
        Assert.Equal(2, value.SubjectId);
    }

    [Fact]
    public async Task ShareRecipe_NotOwner_Returns401()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(UserId, 1, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(RecipeErrors.Unauthorized));

        var result = await _controller.ShareRecipe(1, request);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task ShareRecipe_RecipeNotFound_Returns404()
    {
        var request = new ShareRequestDto(SubjectType.User, 2, Permission.View, null);
        _recipeService.Setup(s => s.ShareRecipeAsync(UserId, 999, request))
            .ReturnsAsync(Result<ResourcePermissionDto>.Failure(RecipeErrors.NotFound));

        var result = await _controller.ShareRecipe(999, request);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
