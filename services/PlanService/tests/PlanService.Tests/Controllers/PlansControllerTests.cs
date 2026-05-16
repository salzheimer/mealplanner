using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlanService.Controllers;
using PlanService.Interfaces;
using PlanService.Models;
using Shared.Models;
using System.Security.Claims;
using Xunit;
using Permission = Shared.Models.Permission;

namespace PlanService.Tests.Controllers;

public class PlansControllerTests
{
    private readonly Mock<IPlanningService> _planningService;
    private readonly PlansController _controller;
    private const int UserId = 1;

    public PlansControllerTests()
    {
        _planningService = new Mock<IPlanningService>();
        _controller = new PlansController(_planningService.Object);
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

    private static PlanSummaryDto MakePlan(int id = 1) => new(
        id, "Weekly Meal Plan", UserId, DateTime.UtcNow, DateTime.UtcNow.AddDays(7)
    );

    private static PlanShareDto MakePlanShare(int id = 1, int planId = 1) => new(
        id, planId, 2, null, UserId, Permission.View, DateTime.UtcNow, null
    );

    // --- GetAllUserPlans ---

    [Fact]
    public async Task GetAllUserPlans_Success_Returns200WithPlans()
    {
        var plans = new List<PlanSummaryDto> { MakePlan(1), MakePlan(2) };
        _planningService.Setup(s => s.GetPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Success(plans));

        var result = await _controller.GetAllUserPlans();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetAllUserPlans_ServiceFailure_Returns404()
    {
        _planningService.Setup(s => s.GetPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetAllUserPlans();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetPlansByStartDate ---

    [Fact]
    public async Task GetPlansByStartDate_Success_Returns200WithPlans()
    {
        var startDate = DateTime.UtcNow;
        var plans = new List<PlanSummaryDto> { MakePlan(1) };
        _planningService.Setup(s => s.GetPlansByStartDateAsync(UserId, startDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Success(plans));

        var result = await _controller.GetPlansByStartDate(startDate);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryDto>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetPlansByStartDate_ServiceFailure_Returns404()
    {
        var startDate = DateTime.UtcNow;
        _planningService.Setup(s => s.GetPlansByStartDateAsync(UserId, startDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetPlansByStartDate(startDate);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetPlansByEndDate ---

    [Fact]
    public async Task GetPlansByEndDate_Success_Returns200WithPlans()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        var plans = new List<PlanSummaryDto> { MakePlan(1) };
        _planningService.Setup(s => s.GetPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Success(plans));

        var result = await _controller.GetPlansByEndDate(endDate);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryDto>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetPlansByEndDate_ServiceFailure_Returns404()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        _planningService.Setup(s => s.GetPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetPlansByEndDate(endDate);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetById ---

    [Fact]
    public async Task GetById_ExistingPlan_Returns200WithPlan()
    {
        _planningService.Setup(s => s.GetPlanByIdAsync(UserId, 1))
            .ReturnsAsync(Result<PlanSummaryDto>.Success(MakePlan(1)));

        var result = await _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanSummaryDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task GetById_NonExistentPlan_Returns404()
    {
        _planningService.Setup(s => s.GetPlanByIdAsync(UserId, 999))
            .ReturnsAsync(Result<PlanSummaryDto>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetById(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetPlansByDateRange ---

    [Fact]
    public async Task GetPlansByDateRange_ValidRange_Returns200WithPlans()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        var plans = new List<PlanSummaryDto> { MakePlan(1), MakePlan(2) };
        _planningService.Setup(s => s.GetPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Success(plans));

        var result = await _controller.GetPlansByDateRange(start, end);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanSummaryDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetPlansByDateRange_InvalidInput_Returns400()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        _planningService.Setup(s => s.GetPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Failure(PlanningErrors.InvalidInput));

        var result = await _controller.GetPlansByDateRange(start, end);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- CreatePlan ---

    [Fact]
    public async Task CreatePlan_ValidPlan_Returns200WithPlan()
    {
        var createDto = new PlanCreateDto("Weekly Plan", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        _planningService.Setup(s => s.CreatePlanAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanSummaryDto>.Success(MakePlan(1)));

        var result = await _controller.CreatePlan(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanSummaryDto>(ok.Value);
        Assert.Equal("Weekly Meal Plan", value.Name);
    }

    [Fact]
    public async Task CreatePlan_ServiceFailure_Returns400()
    {
        var createDto = new PlanCreateDto(null, DateTime.UtcNow, null);
        _planningService.Setup(s => s.CreatePlanAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanSummaryDto>.Failure(PlanningErrors.UnableToCreate));

        var result = await _controller.CreatePlan(createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- UpdatePlan ---

    [Fact]
    public async Task UpdatePlan_ExistingPlan_Returns200WithPlan()
    {
        var updateDto = new PlanUpdateDto(1, "Updated Plan", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        _planningService.Setup(s => s.UpdatePlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanSummaryDto>.Success(MakePlan(1)));

        var result = await _controller.UpdatePlan(updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanSummaryDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdatePlan_NonExistentPlan_Returns400()
    {
        var updateDto = new PlanUpdateDto(999, "Updated Plan", DateTime.UtcNow, null);
        _planningService.Setup(s => s.UpdatePlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanSummaryDto>.Failure(PlanningErrors.UnableToUpdate));

        var result = await _controller.UpdatePlan(updateDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- DeletePlan ---

    [Fact]
    public async Task DeletePlan_ExistingPlan_Returns200()
    {
        _planningService.Setup(s => s.DeletePlanAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeletePlan(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeletePlan_NonExistentPlan_Returns400()
    {
        _planningService.Setup(s => s.DeletePlanAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(PlanningErrors.UnableToDelete));

        var result = await _controller.DeletePlan(999);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- CreatePlanShare ---

    [Fact]
    public async Task CreatePlanShare_ValidShare_Returns200WithShareDto()
    {
        var createDto = new PlanShareCreateDto(0, 1, 2, null, UserId, Permission.View, null);
        _planningService.Setup(s => s.CreatePlanShareAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanShareDto>.Success(MakePlanShare(1, 1)));

        var result = await _controller.CreatePlanShare(createDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanShareDto>(ok.Value);
        Assert.Equal(1, value.PlanId);
    }

    [Fact]
    public async Task CreatePlanShare_ServiceFailure_Returns400()
    {
        var createDto = new PlanShareCreateDto(0, 999, 2, null, UserId, Permission.View, null);
        _planningService.Setup(s => s.CreatePlanShareAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanShareDto>.Failure(PlanningErrors.UnableToShare));

        var result = await _controller.CreatePlanShare(createDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // --- GetPlanSharesByPlanId ---

    [Fact]
    public async Task GetPlanSharesByPlanId_ExistingPlan_Returns200WithShares()
    {
        var shares = new List<PlanShareDto> { MakePlanShare(1, 1), MakePlanShare(2, 1) };
        _planningService.Setup(s => s.GetPlanSharesByPlanIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<PlanShareDto>>.Success(shares));

        var result = await _controller.GetPlanSharesByPlanId(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanShareDto>>(ok.Value);
        Assert.Equal(2, value.Count());
    }

    [Fact]
    public async Task GetPlanSharesByPlanId_NonExistentPlan_Returns404()
    {
        _planningService.Setup(s => s.GetPlanSharesByPlanIdAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<PlanShareDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetPlanSharesByPlanId(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- GetPlanSharesByUserId ---

    [Fact]
    public async Task GetPlanSharesByUserId_Success_Returns200WithShares()
    {
        var shares = new List<PlanShareDto> { MakePlanShare(1, 1) };
        _planningService.Setup(s => s.GetPlanSharesBySharedByUserIdAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanShareDto>>.Success(shares));

        var result = await _controller.GetPlanSharesByUserId();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<PlanShareDto>>(ok.Value);
        Assert.Single(value);
    }

    [Fact]
    public async Task GetPlanSharesByUserId_ServiceFailure_Returns404()
    {
        _planningService.Setup(s => s.GetPlanSharesBySharedByUserIdAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanShareDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetPlanSharesByUserId();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- UpdatePlanShare ---

    [Fact]
    public async Task UpdatePlanShare_ExistingShare_Returns200WithUpdatedShare()
    {
        var updateDto = new PlanShareUpdateDto(1, 1, 2, null, UserId, Permission.Edit, null);
        _planningService.Setup(s => s.UpdatePlanShareAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanShareDto>.Success(MakePlanShare(1, 1)));

        var result = await _controller.UpdatePlanShare(updateDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<PlanShareDto>(ok.Value);
        Assert.Equal(1, value.Id);
    }

    [Fact]
    public async Task UpdatePlanShare_NonExistentShare_Returns404()
    {
        var updateDto = new PlanShareUpdateDto(999, 1, 2, null, UserId, Permission.Edit, null);
        _planningService.Setup(s => s.UpdatePlanShareAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanShareDto>.Failure(PlanningErrors.PlanShareNotFound));

        var result = await _controller.UpdatePlanShare(updateDto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // --- DeletePlanShare ---

    [Fact]
    public async Task DeletePlanShare_ExistingShare_Returns200()
    {
        _planningService.Setup(s => s.DeletePlanShareAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeletePlanShare(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
    }

    [Fact]
    public async Task DeletePlanShare_NonExistentShare_Returns400()
    {
        _planningService.Setup(s => s.DeletePlanShareAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(PlanningErrors.UnableToDelete));

        var result = await _controller.DeletePlanShare(999);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
