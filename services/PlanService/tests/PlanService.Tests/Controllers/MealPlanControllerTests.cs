using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlanService.Controllers;
using PlanService.Interfaces;
using PlanService.Models;
using Shared.Models;
using System.Security.Claims;
using Xunit;

namespace PlanService.Tests.Controllers;

public class MealPlanControllerTests
{
    private readonly Mock<IMealPlanService> _mealPlanService;
    private readonly MealPlanController _controller;
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid SampleMealId = Guid.NewGuid();
    private static readonly Guid SampleMealItemId = Guid.NewGuid();

    public MealPlanControllerTests()
    {
        _mealPlanService = new Mock<IMealPlanService>();
        _controller = new MealPlanController(_mealPlanService.Object);
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

    private static PlanMealResponse MakeMealPlan(Guid? id = null, Guid? planId = null) => new(
        id ?? Guid.NewGuid(), SampleMealId, planId ?? Guid.NewGuid(), DateTime.UtcNow, null, UserId, DateTime.UtcNow, DateTime.UtcNow
    );

    private static PlanMealItemResponse MakeMealItemPlan(Guid? id = null, Guid? mealPlanId = null) => new(
        id ?? Guid.NewGuid(), mealPlanId ?? Guid.NewGuid(), SampleMealItemId, null, null, null, "Pending", null, DateTime.UtcNow, DateTime.UtcNow
    );

    // --- CreateMealPlan ---

    [Fact]
    public async Task CreateMealPlan_ValidData_Returns200WithMealPlan()
    {
        var planId = Guid.NewGuid();
        var createDto = new CreatePlanMealRequest(SampleMealId, planId, DateTime.UtcNow, null, UserId);
        var expected = MakeMealPlan(planId: planId);
        _mealPlanService.Setup(s => s.CreateMealPlanAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanMealResponse?>.Success(expected));

        var result = await _controller.CreateMealPlan(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<PlanMealResponse>(ok.Value);
    }

    [Fact]
    public async Task CreateMealPlan_ServiceFailure_Returns400()
    {
        var createDto = new CreatePlanMealRequest(SampleMealId, Guid.NewGuid(), DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.CreateMealPlanAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanMealResponse?>.Failure(MealPlanErrors.UnableToCreate));

        var result = await _controller.CreateMealPlan(createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetAllUserMealPlans ---

    [Fact]
    public async Task GetAllUserMealPlans_Success_Returns200WithMealPlans()
    {
        var plans = new List<PlanMealResponse> { MakeMealPlan(), MakeMealPlan() };
        _mealPlanService.Setup(s => s.GetMealPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanMealResponse>>.Success(plans));

        var result = await _controller.GetAllUserMealPlans();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanMealResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetAllUserMealPlans_ServiceFailure_Returns404()
    {
        _mealPlanService.Setup(s => s.GetMealPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanMealResponse>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetAllUserMealPlans();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetMealPlanById ---

    [Fact]
    public async Task GetMealPlanById_ExistingId_Returns200WithMealPlan()
    {
        var mealPlanId = Guid.NewGuid();
        _mealPlanService.Setup(s => s.GetMealPlanByIdAsync(UserId, mealPlanId))
            .ReturnsAsync(Result<PlanMealResponse?>.Success(MakeMealPlan(mealPlanId)));

        var result = await _controller.GetMealPlanById(mealPlanId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanMealResponse>(ok.Value);
        Assert.Equal(mealPlanId, value.Id);
    }

    [Fact]
    public async Task GetMealPlanById_NonExistentId_Returns404()
    {
        var mealPlanId = Guid.NewGuid();
        _mealPlanService.Setup(s => s.GetMealPlanByIdAsync(UserId, mealPlanId))
            .ReturnsAsync(Result<PlanMealResponse?>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlanById(mealPlanId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetMealPlansForDate ---

    [Fact]
    public async Task GetMealPlansForDate_ValidDate_Returns200WithMealPlans()
    {
        var date = DateTime.UtcNow;
        var plans = new List<PlanMealResponse> { MakeMealPlan() };
        _mealPlanService.Setup(s => s.GetMealPlansByStartDateAsync(UserId, date))
            .ReturnsAsync(Result<IEnumerable<PlanMealResponse>>.Success(plans));

        var result = await _controller.GetMealPlansForDate(date);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanMealResponse>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetMealPlansForDate_ServiceFailure_Returns404()
    {
        var date = DateTime.UtcNow;
        _mealPlanService.Setup(s => s.GetMealPlansByStartDateAsync(UserId, date))
            .ReturnsAsync(Result<IEnumerable<PlanMealResponse>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlansForDate(date);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetMealPlansForEndDate ---

    [Fact]
    public async Task GetMealPlansForEndDate_ValidDate_Returns200WithMealPlans()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        var plans = new List<PlanMealResponse> { MakeMealPlan() };
        _mealPlanService.Setup(s => s.GetMealPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<PlanMealResponse>>.Success(plans));

        var result = await _controller.GetMealPlansForEndDate(endDate);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanMealResponse>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetMealPlansForEndDate_ServiceFailure_Returns404()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        _mealPlanService.Setup(s => s.GetMealPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<PlanMealResponse>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlansForEndDate(endDate);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetMealPlansForDateRange ---

    [Fact]
    public async Task GetMealPlansForDateRange_ValidRange_Returns200WithMealPlans()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        var plans = new List<PlanMealResponse> { MakeMealPlan(), MakeMealPlan() };
        _mealPlanService.Setup(s => s.GetMealPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<PlanMealResponse>>.Success(plans));

        var result = await _controller.GetMealPlansForDateRange(start, end);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanMealResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetMealPlansForDateRange_InvalidInput_Returns400()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        _mealPlanService.Setup(s => s.GetMealPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<PlanMealResponse>>.Failure(MealPlanErrors.InvalidInput));

        var result = await _controller.GetMealPlansForDateRange(start, end);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- UpdateMealPlan ---

    [Fact]
    public async Task UpdateMealPlan_ExistingPlan_Returns200WithMealPlan()
    {
        var mealPlanId = Guid.NewGuid();
        var updateDto = new UpdatePlanMealRequest(mealPlanId, SampleMealId, Guid.NewGuid(), DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.UpdateMealPlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanMealResponse>.Success(MakeMealPlan(mealPlanId)));

        var result = await _controller.UpdateMealPlan(updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanMealResponse>(ok.Value);
        Assert.Equal(mealPlanId, value.Id);
    }

    [Fact]
    public async Task UpdateMealPlan_NonExistentPlan_Returns400()
    {
        var updateDto = new UpdatePlanMealRequest(Guid.NewGuid(), SampleMealId, Guid.NewGuid(), DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.UpdateMealPlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanMealResponse>.Failure(MealPlanErrors.UnableToUpdate));

        var result = await _controller.UpdateMealPlan(updateDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- DeleteMealPlan ---

    [Fact]
    public async Task DeleteMealPlan_ExistingPlan_Returns200()
    {
        var mealPlanId = Guid.NewGuid();
        _mealPlanService.Setup(s => s.DeleteMealPlanAsync(UserId, mealPlanId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteMealPlan(mealPlanId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteMealPlan_NonExistentPlan_Returns400()
    {
        var mealPlanId = Guid.NewGuid();
        _mealPlanService.Setup(s => s.DeleteMealPlanAsync(UserId, mealPlanId))
            .ReturnsAsync(Result<bool>.Failure(MealPlanErrors.UnableToDelete));

        var result = await _controller.DeleteMealPlan(mealPlanId);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- AddMealItemToPlan ---

    [Fact]
    public async Task AddMealItemToPlan_ValidItem_Returns200WithMealItemPlan()
    {
        var mealPlanId = Guid.NewGuid();
        var createDto = new CreatePlanMealItemRequest(mealPlanId, SampleMealItemId, null, null, null, "Pending", null);
        var expected = MakeMealItemPlan(mealPlanId: mealPlanId);
        _mealPlanService.Setup(s => s.AddMealItemToPlanAsync(UserId, mealPlanId, createDto))
            .ReturnsAsync(Result<PlanMealItemResponse?>.Success(expected));

        var result = await _controller.AddMealItemToPlan(mealPlanId, createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<PlanMealItemResponse>(ok.Value);
    }

    [Fact]
    public async Task AddMealItemToPlan_ServiceFailure_Returns400()
    {
        var mealPlanId = Guid.NewGuid();
        var createDto = new CreatePlanMealItemRequest(mealPlanId, SampleMealItemId, null, null, null, "Pending", null);
        _mealPlanService.Setup(s => s.AddMealItemToPlanAsync(UserId, mealPlanId, createDto))
            .ReturnsAsync(Result<PlanMealItemResponse?>.Failure(MealPlanErrors.UnableToCreate));

        var result = await _controller.AddMealItemToPlan(mealPlanId, createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetMealItemsForPlan ---

    [Fact]
    public async Task GetMealItemsForPlan_ExistingPlan_Returns200WithMealItems()
    {
        var mealPlanId = Guid.NewGuid();
        var items = new List<PlanMealItemResponse> { MakeMealItemPlan(mealPlanId: mealPlanId), MakeMealItemPlan(mealPlanId: mealPlanId) };
        _mealPlanService.Setup(s => s.GetMealItemsForMealPlanAsync(UserId, mealPlanId))
            .ReturnsAsync(Result<IEnumerable<PlanMealItemResponse>>.Success(items));

        var result = await _controller.GetMealItemsForPlan(mealPlanId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanMealItemResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetMealItemsForPlan_NonExistentPlan_Returns404()
    {
        var mealPlanId = Guid.NewGuid();
        _mealPlanService.Setup(s => s.GetMealItemsForMealPlanAsync(UserId, mealPlanId))
            .ReturnsAsync(Result<IEnumerable<PlanMealItemResponse>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealItemsForPlan(mealPlanId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- UpdateMealItemInPlan ---

    [Fact]
    public async Task UpdateMealItemInPlan_ExistingItem_Returns200WithUpdatedItem()
    {
        var mealPlanId = Guid.NewGuid();
        var mealItemId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var updateDto = new UpdatePlanMealItemRequest(itemId, mealPlanId, mealItemId, null, null, null, "Confirmed", null);
        var expected = MakeMealItemPlan(itemId, mealPlanId);
        _mealPlanService.Setup(s => s.UpdateMealItemInPlanAsync(UserId, mealPlanId, mealItemId, updateDto))
            .ReturnsAsync(Result<PlanMealItemResponse>.Success(expected));

        var result = await _controller.UpdateMealItemInPlan(mealPlanId, mealItemId, updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanMealItemResponse>(ok.Value);
        Assert.Equal(itemId, value.Id);
    }

    [Fact]
    public async Task UpdateMealItemInPlan_NonExistentItem_Returns400()
    {
        var mealPlanId = Guid.NewGuid();
        var mealItemId = Guid.NewGuid();
        var updateDto = new UpdatePlanMealItemRequest(Guid.NewGuid(), mealPlanId, mealItemId, null, null, null, "Confirmed", null);
        _mealPlanService.Setup(s => s.UpdateMealItemInPlanAsync(UserId, mealPlanId, mealItemId, updateDto))
            .ReturnsAsync(Result<PlanMealItemResponse>.Failure(MealPlanErrors.UnableToUpdate));

        var result = await _controller.UpdateMealItemInPlan(mealPlanId, mealItemId, updateDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- RemoveMealItemFromPlan ---

    [Fact]
    public async Task RemoveMealItemFromPlan_ExistingItem_Returns200()
    {
        var mealItemPlanId = Guid.NewGuid();
        _mealPlanService.Setup(s => s.RemoveMealItemFromPlanAsync(UserId, mealItemPlanId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.RemoveMealItemFromPlan(mealItemPlanId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task RemoveMealItemFromPlan_NonExistentItem_Returns400()
    {
        var mealItemPlanId = Guid.NewGuid();
        _mealPlanService.Setup(s => s.RemoveMealItemFromPlanAsync(UserId, mealItemPlanId))
            .ReturnsAsync(Result<bool>.Failure(MealPlanErrors.UnableToDelete));

        var result = await _controller.RemoveMealItemFromPlan(mealItemPlanId);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
