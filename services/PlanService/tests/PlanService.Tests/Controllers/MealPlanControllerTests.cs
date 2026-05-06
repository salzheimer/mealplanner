using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlanService.Controllers;
using PlanService.Interfaces;
using PlanService.Models;
using Shared.Models;
using System.Security.Claims;
using Xunit;
using ItemStatus = Shared.Models.ItemStatus;

namespace PlanService.Tests.Controllers;

public class MealPlanControllerTests
{
    private readonly Mock<IMealPlanService> _mealPlanService;
    private readonly MealPlanController _controller;
    private const int UserId = 1;

    public MealPlanControllerTests()
    {
        _mealPlanService = new Mock<IMealPlanService>();
        _controller = new MealPlanController(_mealPlanService.Object);
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

    private static MealPlanDto MakeMealPlan(int id = 1, int planId = 1) => new(
        id, MealId: 10, planId, DateTime.UtcNow, null, UserId, DateTime.UtcNow, DateTime.UtcNow
    );

    private static MealItemPlanDto MakeMealItemPlan(int id = 1, int mealPlanId = 1) => new(
        id, mealPlanId, MealItemId: 5, null, null, ItemStatus.Pending, null, DateTime.UtcNow, DateTime.UtcNow
    );

    // --- CreateMealPlan ---

    [Fact]
    public async Task CreateMealPlan_ValidData_ReturnsMealPlan()
    {
        var createDto = new MealPlanCreateDto(10, 1, DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.CreateMealPlanAsync(UserId, createDto))
            .ReturnsAsync(Result<MealPlanDto?>.Success(MakeMealPlan(1, 1)));

        var result = await _controller.CreateMealPlan(createDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task CreateMealPlan_ServiceFailure_ReturnsFailure()
    {
        var createDto = new MealPlanCreateDto(10, 999, DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.CreateMealPlanAsync(UserId, createDto))
            .ReturnsAsync(Result<MealPlanDto?>.Failure(MealPlanErrors.UnableToCreate));

        var result = await _controller.CreateMealPlan(createDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- GetAllUserMealPlans ---

    [Fact]
    public async Task GetAllUserMealPlans_Success_ReturnsMealPlans()
    {
        var plans = new List<MealPlanDto> { MakeMealPlan(1), MakeMealPlan(2) };
        _mealPlanService.Setup(s => s.GetMealPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Success(plans));

        var result = await _controller.GetAllUserMealPlans();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetAllUserMealPlans_ServiceFailure_ReturnsFailure()
    {
        _mealPlanService.Setup(s => s.GetMealPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetAllUserMealPlans();

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.MealPlanNotFound.Code, result.Error.Code);
    }

    // --- GetMealPlanById ---

    [Fact]
    public async Task GetMealPlanById_ExistingId_ReturnsMealPlan()
    {
        _mealPlanService.Setup(s => s.GetMealPlanByIdAsync(UserId, 1))
            .ReturnsAsync(Result<MealPlanDto?>.Success(MakeMealPlan(1)));

        var result = await _controller.GetMealPlanById(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task GetMealPlanById_NonExistentId_ReturnsFailure()
    {
        _mealPlanService.Setup(s => s.GetMealPlanByIdAsync(UserId, 999))
            .ReturnsAsync(Result<MealPlanDto?>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlanById(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.MealPlanNotFound.Code, result.Error.Code);
    }

    // --- GetMealPlansForDate ---

    [Fact]
    public async Task GetMealPlansForDate_ValidDate_ReturnsMealPlans()
    {
        var date = DateTime.UtcNow;
        var plans = new List<MealPlanDto> { MakeMealPlan(1) };
        _mealPlanService.Setup(s => s.GetMealPlansByStartDateAsync(UserId, date))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Success(plans));

        var result = await _controller.GetMealPlansForDate(date);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task GetMealPlansForDate_ServiceFailure_ReturnsFailure()
    {
        var date = DateTime.UtcNow;
        _mealPlanService.Setup(s => s.GetMealPlansByStartDateAsync(UserId, date))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlansForDate(date);

        Assert.False(result.IsSuccess);
    }

    // --- GetMealPlansForEndDate ---

    [Fact]
    public async Task GetMealPlansForEndDate_ValidDate_ReturnsMealPlans()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        var plans = new List<MealPlanDto> { MakeMealPlan(1) };
        _mealPlanService.Setup(s => s.GetMealPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Success(plans));

        var result = await _controller.GetMealPlansForEndDate(endDate);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task GetMealPlansForEndDate_ServiceFailure_ReturnsFailure()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        _mealPlanService.Setup(s => s.GetMealPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlansForEndDate(endDate);

        Assert.False(result.IsSuccess);
    }

    // --- GetMealPlansForDateRange ---

    [Fact]
    public async Task GetMealPlansForDateRange_ValidRange_ReturnsMealPlans()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        var plans = new List<MealPlanDto> { MakeMealPlan(1), MakeMealPlan(2) };
        _mealPlanService.Setup(s => s.GetMealPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Success(plans));

        var result = await _controller.GetMealPlansForDateRange(start, end);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetMealPlansForDateRange_ServiceFailure_ReturnsFailure()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        _mealPlanService.Setup(s => s.GetMealPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Failure(MealPlanErrors.InvalidInput));

        var result = await _controller.GetMealPlansForDateRange(start, end);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.InvalidInput.Code, result.Error.Code);
    }

    // --- UpdateMealPlan ---

    [Fact]
    public async Task UpdateMealPlan_ExistingPlan_ReturnsMealPlan()
    {
        var updateDto = new MealPlanUpdateDto(1, 10, 1, DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.UpdateMealPlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealPlanDto>.Success(MakeMealPlan(1)));

        var result = await _controller.UpdateMealPlan(updateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task UpdateMealPlan_NonExistentPlan_ReturnsFailure()
    {
        var updateDto = new MealPlanUpdateDto(999, 10, 1, DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.UpdateMealPlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealPlanDto>.Failure(MealPlanErrors.UnableToUpdate));

        var result = await _controller.UpdateMealPlan(updateDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.UnableToUpdate.Code, result.Error.Code);
    }

    // --- DeleteMealPlan ---

    [Fact]
    public async Task DeleteMealPlan_ExistingPlan_ReturnsSuccess()
    {
        _mealPlanService.Setup(s => s.DeleteMealPlanAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteMealPlan(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task DeleteMealPlan_NonExistentPlan_ReturnsFailure()
    {
        _mealPlanService.Setup(s => s.DeleteMealPlanAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(MealPlanErrors.UnableToDelete));

        var result = await _controller.DeleteMealPlan(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.UnableToDelete.Code, result.Error.Code);
    }

    // --- AddMealItemToPlan ---

    [Fact]
    public async Task AddMealItemToPlan_ValidItem_ReturnsMealItemPlan()
    {
        var createDto = new MealItemPlanCreateDto(1, 5, null, null, ItemStatus.Pending, null);
        _mealPlanService.Setup(s => s.AddMealItemToPlanAsync(UserId, createDto))
            .ReturnsAsync(Result<MealItemPlanDto?>.Success(MakeMealItemPlan(1, 1)));

        var result = await _controller.AddMealItemToPlan(createDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task AddMealItemToPlan_ServiceFailure_ReturnsFailure()
    {
        var createDto = new MealItemPlanCreateDto(999, 5, null, null, null, null);
        _mealPlanService.Setup(s => s.AddMealItemToPlanAsync(UserId, createDto))
            .ReturnsAsync(Result<MealItemPlanDto?>.Failure(MealPlanErrors.UnableToCreate));

        var result = await _controller.AddMealItemToPlan(createDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- GetMealItemsForPlan ---

    [Fact]
    public async Task GetMealItemsForPlan_ExistingPlan_ReturnsMealItems()
    {
        var items = new List<MealItemPlanDto> { MakeMealItemPlan(1, 1), MakeMealItemPlan(2, 1) };
        _mealPlanService.Setup(s => s.GetMealItemsForMealPlanAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<MealItemPlanDto>>.Success(items));

        var result = await _controller.GetMealItemsForPlan(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetMealItemsForPlan_NonExistentPlan_ReturnsFailure()
    {
        _mealPlanService.Setup(s => s.GetMealItemsForMealPlanAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<MealItemPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealItemsForPlan(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.MealPlanNotFound.Code, result.Error.Code);
    }

    // --- UpdateMealItemInPlan ---

    [Fact]
    public async Task UpdateMealItemInPlan_ExistingItem_ReturnsUpdatedItem()
    {
        var updateDto = new MealItemPlanUpdateDto(1, 1, 5, null, null, ItemStatus.Confirmed, null);
        _mealPlanService.Setup(s => s.UpdateMealItemInPlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealItemPlanDto>.Success(MakeMealItemPlan(1, 1)));

        var result = await _controller.UpdateMealItemInPlan(updateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task UpdateMealItemInPlan_NonExistentItem_ReturnsFailure()
    {
        var updateDto = new MealItemPlanUpdateDto(999, 1, 5, null, null, null, null);
        _mealPlanService.Setup(s => s.UpdateMealItemInPlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealItemPlanDto>.Failure(MealPlanErrors.UnableToUpdate));

        var result = await _controller.UpdateMealItemInPlan(updateDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.UnableToUpdate.Code, result.Error.Code);
    }

    // --- RemoveMealItemFromPlan ---

    [Fact]
    public async Task RemoveMealItemFromPlan_ExistingItem_ReturnsSuccess()
    {
        _mealPlanService.Setup(s => s.RemoveMealItemFromPlanAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.RemoveMealItemFromPlan(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task RemoveMealItemFromPlan_NonExistentItem_ReturnsFailure()
    {
        _mealPlanService.Setup(s => s.RemoveMealItemFromPlanAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(MealPlanErrors.UnableToDelete));

        var result = await _controller.RemoveMealItemFromPlan(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(MealPlanErrors.UnableToDelete.Code, result.Error.Code);
    }
}
