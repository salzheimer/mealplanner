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
    public async Task CreateMealPlan_ValidData_Returns200WithMealPlan()
    {
        var createDto = new MealPlanCreateDto(10, 1, DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.CreateMealPlanAsync(UserId, createDto))
            .ReturnsAsync(Result<MealPlanDto?>.Success(MakeMealPlan(1, 1)));

        var result = await _controller.CreateMealPlan(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealPlanDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task CreateMealPlan_ServiceFailure_Returns400()
    {
        var createDto = new MealPlanCreateDto(10, 999, DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.CreateMealPlanAsync(UserId, createDto))
            .ReturnsAsync(Result<MealPlanDto?>.Failure(MealPlanErrors.UnableToCreate));

        var result = await _controller.CreateMealPlan(createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetAllUserMealPlans ---

    [Fact]
    public async Task GetAllUserMealPlans_Success_Returns200WithMealPlans()
    {
        var plans = new List<MealPlanDto> { MakeMealPlan(1), MakeMealPlan(2) };
        _mealPlanService.Setup(s => s.GetMealPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Success(plans));

        var result = await _controller.GetAllUserMealPlans();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<MealPlanDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetAllUserMealPlans_ServiceFailure_Returns404()
    {
        _mealPlanService.Setup(s => s.GetMealPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetAllUserMealPlans();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetMealPlanById ---

    [Fact]
    public async Task GetMealPlanById_ExistingId_Returns200WithMealPlan()
    {
        _mealPlanService.Setup(s => s.GetMealPlanByIdAsync(UserId, 1))
            .ReturnsAsync(Result<MealPlanDto?>.Success(MakeMealPlan(1)));

        var result = await _controller.GetMealPlanById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealPlanDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task GetMealPlanById_NonExistentId_Returns404()
    {
        _mealPlanService.Setup(s => s.GetMealPlanByIdAsync(UserId, 999))
            .ReturnsAsync(Result<MealPlanDto?>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlanById(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetMealPlansForDate ---

    [Fact]
    public async Task GetMealPlansForDate_ValidDate_Returns200WithMealPlans()
    {
        var date = DateTime.UtcNow;
        var plans = new List<MealPlanDto> { MakeMealPlan(1) };
        _mealPlanService.Setup(s => s.GetMealPlansByStartDateAsync(UserId, date))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Success(plans));

        var result = await _controller.GetMealPlansForDate(date);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<MealPlanDto>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetMealPlansForDate_ServiceFailure_Returns404()
    {
        var date = DateTime.UtcNow;
        _mealPlanService.Setup(s => s.GetMealPlansByStartDateAsync(UserId, date))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlansForDate(date);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetMealPlansForEndDate ---

    [Fact]
    public async Task GetMealPlansForEndDate_ValidDate_Returns200WithMealPlans()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        var plans = new List<MealPlanDto> { MakeMealPlan(1) };
        _mealPlanService.Setup(s => s.GetMealPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Success(plans));

        var result = await _controller.GetMealPlansForEndDate(endDate);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<MealPlanDto>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetMealPlansForEndDate_ServiceFailure_Returns404()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        _mealPlanService.Setup(s => s.GetMealPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealPlansForEndDate(endDate);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetMealPlansForDateRange ---

    [Fact]
    public async Task GetMealPlansForDateRange_ValidRange_Returns200WithMealPlans()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        var plans = new List<MealPlanDto> { MakeMealPlan(1), MakeMealPlan(2) };
        _mealPlanService.Setup(s => s.GetMealPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Success(plans));

        var result = await _controller.GetMealPlansForDateRange(start, end);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<MealPlanDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetMealPlansForDateRange_InvalidInput_Returns400()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        _mealPlanService.Setup(s => s.GetMealPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<MealPlanDto>>.Failure(MealPlanErrors.InvalidInput));

        var result = await _controller.GetMealPlansForDateRange(start, end);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- UpdateMealPlan ---

    [Fact]
    public async Task UpdateMealPlan_ExistingPlan_Returns200WithMealPlan()
    {
        var updateDto = new MealPlanUpdateDto(1, 10, 1, DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.UpdateMealPlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealPlanDto>.Success(MakeMealPlan(1)));

        var result = await _controller.UpdateMealPlan(updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealPlanDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdateMealPlan_NonExistentPlan_Returns400()
    {
        var updateDto = new MealPlanUpdateDto(999, 10, 1, DateTime.UtcNow, null, UserId);
        _mealPlanService.Setup(s => s.UpdateMealPlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<MealPlanDto>.Failure(MealPlanErrors.UnableToUpdate));

        var result = await _controller.UpdateMealPlan(updateDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- DeleteMealPlan ---

    [Fact]
    public async Task DeleteMealPlan_ExistingPlan_Returns200()
    {
        _mealPlanService.Setup(s => s.DeleteMealPlanAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeleteMealPlan(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeleteMealPlan_NonExistentPlan_Returns400()
    {
        _mealPlanService.Setup(s => s.DeleteMealPlanAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(MealPlanErrors.UnableToDelete));

        var result = await _controller.DeleteMealPlan(999);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- AddMealItemToPlan ---

    [Fact]
    public async Task AddMealItemToPlan_ValidItem_Returns200WithMealItemPlan()
    {
        var createDto = new MealItemPlanCreateDto(1, 5, null, null, ItemStatus.Pending, null);
        _mealPlanService.Setup(s => s.AddMealItemToPlanAsync(UserId, 1, createDto))
            .ReturnsAsync(Result<MealItemPlanDto?>.Success(MakeMealItemPlan(1, 1)));

        var result = await _controller.AddMealItemToPlan(1, createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealItemPlanDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task AddMealItemToPlan_ServiceFailure_Returns400()
    {
        var createDto = new MealItemPlanCreateDto(999, 5, null, null, null, null);
        _mealPlanService.Setup(s => s.AddMealItemToPlanAsync(UserId, 999, createDto))
            .ReturnsAsync(Result<MealItemPlanDto?>.Failure(MealPlanErrors.UnableToCreate));

        var result = await _controller.AddMealItemToPlan(999, createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetMealItemsForPlan ---

    [Fact]
    public async Task GetMealItemsForPlan_ExistingPlan_Returns200WithMealItems()
    {
        var items = new List<MealItemPlanDto> { MakeMealItemPlan(1, 1), MakeMealItemPlan(2, 1) };
        _mealPlanService.Setup(s => s.GetMealItemsForMealPlanAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<MealItemPlanDto>>.Success(items));

        var result = await _controller.GetMealItemsForPlan(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<MealItemPlanDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetMealItemsForPlan_NonExistentPlan_Returns404()
    {
        _mealPlanService.Setup(s => s.GetMealItemsForMealPlanAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<MealItemPlanDto>>.Failure(MealPlanErrors.MealPlanNotFound));

        var result = await _controller.GetMealItemsForPlan(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- UpdateMealItemInPlan ---

    [Fact]
    public async Task UpdateMealItemInPlan_ExistingItem_Returns200WithUpdatedItem()
    {
        var updateDto = new MealItemPlanUpdateDto(1, 1, 5, null, null, ItemStatus.Confirmed, null);
        _mealPlanService.Setup(s => s.UpdateMealItemInPlanAsync(UserId, 1, 5, updateDto))
            .ReturnsAsync(Result<MealItemPlanDto>.Success(MakeMealItemPlan(1, 1)));

        var result = await _controller.UpdateMealItemInPlan(1, 5, updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<MealItemPlanDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdateMealItemInPlan_NonExistentItem_Returns400()
    {
        var updateDto = new MealItemPlanUpdateDto(999, 1, 5, null, null, null, null);
        _mealPlanService.Setup(s => s.UpdateMealItemInPlanAsync(UserId, 1, 5, updateDto))
            .ReturnsAsync(Result<MealItemPlanDto>.Failure(MealPlanErrors.UnableToUpdate));

        var result = await _controller.UpdateMealItemInPlan(1, 5, updateDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- RemoveMealItemFromPlan ---

    [Fact]
    public async Task RemoveMealItemFromPlan_ExistingItem_Returns200()
    {
        _mealPlanService.Setup(s => s.RemoveMealItemFromPlanAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.RemoveMealItemFromPlan(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task RemoveMealItemFromPlan_NonExistentItem_Returns400()
    {
        _mealPlanService.Setup(s => s.RemoveMealItemFromPlanAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(MealPlanErrors.UnableToDelete));

        var result = await _controller.RemoveMealItemFromPlan(999);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
