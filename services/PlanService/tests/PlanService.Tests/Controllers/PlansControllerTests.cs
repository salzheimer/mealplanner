using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlanService.Controllers;
using PlanService.Contracts;
using PlanService.Interfaces;
using PlanService.Models;
using Shared.Models;
using System.Security.Claims;
using Xunit;

namespace PlanService.Tests.Controllers;

public class PlansControllerTests
{
    private readonly Mock<IPlanningService> _planningService;
    private readonly PlansController _controller;
    private static readonly Guid UserId = Guid.NewGuid();

    public PlansControllerTests()
    {
        _planningService = new Mock<IPlanningService>();
        _controller = new PlansController(_planningService.Object);
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

    private static PlanSummaryResponse MakePlan(Guid? id = null) => new(
        id ?? Guid.NewGuid(), "Weekly Meal Plan", UserId, DateTime.UtcNow, DateTime.UtcNow.AddDays(7)
    );

    private static SharePlanResponse MakeShareResponse(Guid planId) => new(
        planId, "Plan", 1, "User", 1, "View", 1, Guid.NewGuid(), UserId, null
    );

    // --- GetAllUserPlans ---

    [Fact]
    public async Task GetAllUserPlans_Success_Returns200WithPlans()
    {
        var plans = new List<PlanSummaryResponse> { MakePlan(), MakePlan() };
        _planningService.Setup(s => s.GetPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Success(plans));

        var result = await _controller.GetAllUserPlans();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetAllUserPlans_ServiceFailure_Returns404()
    {
        _planningService.Setup(s => s.GetPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Failure(PlanErrors.PlanNotFound));

        var result = await _controller.GetAllUserPlans();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetPlansByStartDate ---

    [Fact]
    public async Task GetPlansByStartDate_Success_Returns200WithPlans()
    {
        var startDate = DateTime.UtcNow;
        var plans = new List<PlanSummaryResponse> { MakePlan() };
        _planningService.Setup(s => s.GetPlansByStartDateAsync(UserId, startDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Success(plans));

        var result = await _controller.GetPlansByStartDate(startDate);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryResponse>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetPlansByStartDate_ServiceFailure_Returns404()
    {
        var startDate = DateTime.UtcNow;
        _planningService.Setup(s => s.GetPlansByStartDateAsync(UserId, startDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Failure(PlanErrors.PlanNotFound));

        var result = await _controller.GetPlansByStartDate(startDate);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetPlansByEndDate ---

    [Fact]
    public async Task GetPlansByEndDate_Success_Returns200WithPlans()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        var plans = new List<PlanSummaryResponse> { MakePlan() };
        _planningService.Setup(s => s.GetPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Success(plans));

        var result = await _controller.GetPlansByEndDate(endDate);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryResponse>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetPlansByEndDate_ServiceFailure_Returns404()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        _planningService.Setup(s => s.GetPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Failure(PlanErrors.PlanNotFound));

        var result = await _controller.GetPlansByEndDate(endDate);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetById ---

    [Fact]
    public async Task GetById_ExistingPlan_Returns200WithPlan()
    {
        var planId = Guid.NewGuid();
        _planningService.Setup(s => s.GetPlanByIdAsync(UserId, planId))
            .ReturnsAsync(Result<PlanSummaryResponse>.Success(MakePlan(planId)));

        var result = await _controller.GetById(planId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanSummaryResponse>(ok.Value);
        Assert.Equal(planId, value.Id);
    }

    [Fact]
    public async Task GetById_NonExistentPlan_Returns404()
    {
        var planId = Guid.NewGuid();
        _planningService.Setup(s => s.GetPlanByIdAsync(UserId, planId))
            .ReturnsAsync(Result<PlanSummaryResponse>.Failure(PlanErrors.PlanNotFound));

        var result = await _controller.GetById(planId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetPlansByDateRange ---

    [Fact]
    public async Task GetPlansByDateRange_ValidRange_Returns200WithPlans()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        var plans = new List<PlanSummaryResponse> { MakePlan(), MakePlan() };
        _planningService.Setup(s => s.GetPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Success(plans));

        var result = await _controller.GetPlansByDateRange(start, end);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryResponse>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetPlansByDateRange_InvalidInput_Returns400()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        _planningService.Setup(s => s.GetPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Failure(PlanErrors.InvalidInput));

        var result = await _controller.GetPlansByDateRange(start, end);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- PlansSharedWithMe ---

    [Fact]
    public async Task PlansSharedWithMe_Success_Returns200WithPlans()
    {
        var plans = new List<PlanSummaryResponse> { MakePlan() };
        _planningService.Setup(s => s.GetPlansSharedWithMeAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Success(plans));

        var result = await _controller.PlansSharedWithMe();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryResponse>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task PlansSharedWithMe_ServiceFailure_Returns404()
    {
        _planningService.Setup(s => s.GetPlansSharedWithMeAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryResponse>>.Failure(PlanErrors.PlanNotFound));

        var result = await _controller.PlansSharedWithMe();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- CreatePlan ---

    [Fact]
    public async Task CreatePlan_ValidPlan_Returns200WithPlan()
    {
        var createDto = new CreatePlanRequest("Weekly Plan", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        _planningService.Setup(s => s.CreatePlanAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanSummaryResponse>.Success(MakePlan()));

        var result = await _controller.CreatePlan(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanSummaryResponse>(ok.Value);
        Assert.Equal("Weekly Meal Plan", value.Name);
    }

    [Fact]
    public async Task CreatePlan_ServiceFailure_Returns400()
    {
        var createDto = new CreatePlanRequest(null, DateTime.UtcNow, null);
        _planningService.Setup(s => s.CreatePlanAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanSummaryResponse>.Failure(PlanErrors.UnableToCreate));

        var result = await _controller.CreatePlan(createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- UpdatePlan ---

    [Fact]
    public async Task UpdatePlan_ExistingPlan_Returns200WithPlan()
    {
        var planId = Guid.NewGuid();
        var updateDto = new UpdatePlanRequest(planId, "Updated Plan", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        _planningService.Setup(s => s.UpdatePlanAsync(UserId, planId, updateDto))
            .ReturnsAsync(Result<PlanSummaryResponse>.Success(MakePlan(planId)));

        var result = await _controller.UpdatePlan(planId, updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanSummaryResponse>(ok.Value);
        Assert.Equal(planId, value.Id);
    }

    [Fact]
    public async Task UpdatePlan_NonExistentPlan_Returns400()
    {
        var planId = Guid.NewGuid();
        var updateDto = new UpdatePlanRequest(planId, "Updated Plan", DateTime.UtcNow, null);
        _planningService.Setup(s => s.UpdatePlanAsync(UserId, planId, updateDto))
            .ReturnsAsync(Result<PlanSummaryResponse>.Failure(PlanErrors.UnableToUpdate));

        var result = await _controller.UpdatePlan(planId, updateDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- DeletePlan ---

    [Fact]
    public async Task DeletePlan_ExistingPlan_Returns200()
    {
        var planId = Guid.NewGuid();
        _planningService.Setup(s => s.DeletePlanAsync(UserId, planId))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeletePlan(planId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeletePlan_NonExistentPlan_Returns400()
    {
        var planId = Guid.NewGuid();
        _planningService.Setup(s => s.DeletePlanAsync(UserId, planId))
            .ReturnsAsync(Result<bool>.Failure(PlanErrors.UnableToDelete));

        var result = await _controller.DeletePlan(planId);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- SharePlan ---

    [Fact]
    public async Task SharePlan_ValidRequest_Returns200WithShareResponse()
    {
        var planId = Guid.NewGuid();
        var shareRequest = new SharePlanRequest(planId, "User", Guid.NewGuid(), "View", UserId, null);
        _planningService.Setup(s => s.SharePlanAsync(UserId, shareRequest))
            .ReturnsAsync(Result<SharePlanResponse>.Success(MakeShareResponse(planId)));

        var result = await _controller.SharePlan(shareRequest);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<SharePlanResponse>(ok.Value);
        Assert.Equal(planId, value.PlanId);
    }

    [Fact]
    public async Task SharePlan_ServiceFailure_Returns400()
    {
        var shareRequest = new SharePlanRequest(Guid.NewGuid(), "User", Guid.NewGuid(), "View", UserId, null);
        _planningService.Setup(s => s.SharePlanAsync(UserId, shareRequest))
            .ReturnsAsync(Result<SharePlanResponse>.Failure(PlanErrors.UnableToShare));

        var result = await _controller.SharePlan(shareRequest);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
