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
    public async Task GetAllUserPlans_Success_ReturnsPlans()
    {
        var plans = new List<PlanSummaryDto> { MakePlan(1), MakePlan(2) };
        _planningService.Setup(s => s.GetPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Success(plans));

        var result = await _controller.GetAllUserPlans();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetAllUserPlans_ServiceFailure_ReturnsFailure()
    {
        _planningService.Setup(s => s.GetPlansForUserAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetAllUserPlans();

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.PlanNotFound.Code, result.Error.Code);
    }

    // --- GetPlansByStartDate ---

    [Fact]
    public async Task GetPlansByStartDate_Success_ReturnsPlans()
    {
        var startDate = DateTime.UtcNow;
        var plans = new List<PlanSummaryDto> { MakePlan(1) };
        _planningService.Setup(s => s.GetPlansByStartDateAsync(UserId, startDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Success(plans));

        var result = await _controller.GetPlansByStartDate(startDate);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task GetPlansByStartDate_ServiceFailure_ReturnsFailure()
    {
        var startDate = DateTime.UtcNow;
        _planningService.Setup(s => s.GetPlansByStartDateAsync(UserId, startDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetPlansByStartDate(startDate);

        Assert.False(result.IsSuccess);
    }

    // --- GetPlansByEndDate ---

    [Fact]
    public async Task GetPlansByEndDate_Success_ReturnsPlans()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        var plans = new List<PlanSummaryDto> { MakePlan(1) };
        _planningService.Setup(s => s.GetPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Success(plans));

        var result = await _controller.GetPlansByEndDate(endDate);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task GetPlansByEndDate_ServiceFailure_ReturnsFailure()
    {
        var endDate = DateTime.UtcNow.AddDays(7);
        _planningService.Setup(s => s.GetPlansByEndDateAsync(UserId, endDate))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetPlansByEndDate(endDate);

        Assert.False(result.IsSuccess);
    }

    // --- GetById ---

    [Fact]
    public async Task GetById_ExistingPlan_ReturnsPlan()
    {
        _planningService.Setup(s => s.GetPlanByIdAsync(1))
            .ReturnsAsync(Result<PlanSummaryDto>.Success(MakePlan(1)));

        var result = await _controller.GetById(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task GetById_NonExistentPlan_ReturnsFailure()
    {
        _planningService.Setup(s => s.GetPlanByIdAsync(999))
            .ReturnsAsync(Result<PlanSummaryDto>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetById(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.PlanNotFound.Code, result.Error.Code);
    }

    // --- GetPlansByDateRange ---

    [Fact]
    public async Task GetPlansByDateRange_ValidRange_ReturnsPlans()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        var plans = new List<PlanSummaryDto> { MakePlan(1), MakePlan(2) };
        _planningService.Setup(s => s.GetPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Success(plans));

        var result = await _controller.GetPlansByDateRange(start, end);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetPlansByDateRange_ServiceFailure_ReturnsFailure()
    {
        var start = DateTime.UtcNow;
        var end = DateTime.UtcNow.AddDays(7);
        _planningService.Setup(s => s.GetPlansByDateRangeAsync(UserId, start, end))
            .ReturnsAsync(Result<IEnumerable<PlanSummaryDto>>.Failure(PlanningErrors.InvalidInput));

        var result = await _controller.GetPlansByDateRange(start, end);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.InvalidInput.Code, result.Error.Code);
    }

    // --- CreatePlan ---

    [Fact]
    public async Task CreatePlan_ValidPlan_ReturnsPlan()
    {
        var createDto = new PlanCreateDto("Weekly Plan", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        _planningService.Setup(s => s.CreatePlanAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanSummaryDto>.Success(MakePlan(1)));

        var result = await _controller.CreatePlan(createDto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Weekly Meal Plan", result.Value!.Name);
    }

    [Fact]
    public async Task CreatePlan_ServiceFailure_ReturnsFailure()
    {
        var createDto = new PlanCreateDto(null, DateTime.UtcNow, null);
        _planningService.Setup(s => s.CreatePlanAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanSummaryDto>.Failure(PlanningErrors.UnableToCreate));

        var result = await _controller.CreatePlan(createDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.UnableToCreate.Code, result.Error.Code);
    }

    // --- UpdatePlan ---

    [Fact]
    public async Task UpdatePlan_ExistingPlan_ReturnsPlan()
    {
        var updateDto = new PlanUpdateDto(1, "Updated Plan", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        _planningService.Setup(s => s.UpdatePlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanSummaryDto>.Success(MakePlan(1)));

        var result = await _controller.UpdatePlan(updateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task UpdatePlan_NonExistentPlan_ReturnsFailure()
    {
        var updateDto = new PlanUpdateDto(999, "Updated Plan", DateTime.UtcNow, null);
        _planningService.Setup(s => s.UpdatePlanAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanSummaryDto>.Failure(PlanningErrors.UnableToUpdate));

        var result = await _controller.UpdatePlan(updateDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.UnableToUpdate.Code, result.Error.Code);
    }

    // --- DeletePlan ---

    [Fact]
    public async Task DeletePlan_ExistingPlan_ReturnsSuccess()
    {
        _planningService.Setup(s => s.DeletePlanAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeletePlan(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task DeletePlan_NonExistentPlan_ReturnsFailure()
    {
        _planningService.Setup(s => s.DeletePlanAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(PlanningErrors.UnableToDelete));

        var result = await _controller.DeletePlan(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.UnableToDelete.Code, result.Error.Code);
    }

    // --- CreatePlanShare ---

    [Fact]
    public async Task CreatePlanShare_ValidShare_ReturnsShareDto()
    {
        var createDto = new PlanShareCreateDto(0, 1, 2, null, UserId, Permission.View, null);
        _planningService.Setup(s => s.CreatePlanShareAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanShareDto>.Success(MakePlanShare(1, 1)));

        var result = await _controller.CreatePlanShare(createDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.PlanId);
    }

    [Fact]
    public async Task CreatePlanShare_ServiceFailure_ReturnsFailure()
    {
        var createDto = new PlanShareCreateDto(0, 999, 2, null, UserId, Permission.View, null);
        _planningService.Setup(s => s.CreatePlanShareAsync(UserId, createDto))
            .ReturnsAsync(Result<PlanShareDto>.Failure(PlanningErrors.UnableToShare));

        var result = await _controller.CreatePlanShare(createDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.UnableToShare.Code, result.Error.Code);
    }

    // --- GetPlanSharesByPlanId ---

    [Fact]
    public async Task GetPlanSharesByPlanId_ExistingPlan_ReturnsShares()
    {
        var shares = new List<PlanShareDto> { MakePlanShare(1, 1), MakePlanShare(2, 1) };
        _planningService.Setup(s => s.GetPlanSharesByPlanIdAsync(UserId, 1))
            .ReturnsAsync(Result<IEnumerable<PlanShareDto>>.Success(shares));

        var result = await _controller.GetPlanSharesByPlanId(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count());
    }

    [Fact]
    public async Task GetPlanSharesByPlanId_NonExistentPlan_ReturnsFailure()
    {
        _planningService.Setup(s => s.GetPlanSharesByPlanIdAsync(UserId, 999))
            .ReturnsAsync(Result<IEnumerable<PlanShareDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetPlanSharesByPlanId(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.PlanNotFound.Code, result.Error.Code);
    }

    // --- GetPlanSharesByUserId ---

    [Fact]
    public async Task GetPlanSharesByUserId_Success_ReturnsShares()
    {
        var shares = new List<PlanShareDto> { MakePlanShare(1, 1) };
        _planningService.Setup(s => s.GetPlanSharesBySharedByUserIdAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanShareDto>>.Success(shares));

        var result = await _controller.GetPlanSharesByUserId();

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
    }

    [Fact]
    public async Task GetPlanSharesByUserId_ServiceFailure_ReturnsFailure()
    {
        _planningService.Setup(s => s.GetPlanSharesBySharedByUserIdAsync(UserId))
            .ReturnsAsync(Result<IEnumerable<PlanShareDto>>.Failure(PlanningErrors.PlanNotFound));

        var result = await _controller.GetPlanSharesByUserId();

        Assert.False(result.IsSuccess);
    }

    // --- UpdatePlanShare ---

    [Fact]
    public async Task UpdatePlanShare_ExistingShare_ReturnsUpdatedShare()
    {
        var updateDto = new PlanShareUpdateDto(1, 1, 2, null, UserId, Permission.Edit, null);
        _planningService.Setup(s => s.UpdatePlanShareAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanShareDto>.Success(MakePlanShare(1, 1)));

        var result = await _controller.UpdatePlanShare(updateDto);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value!.Id);
    }

    [Fact]
    public async Task UpdatePlanShare_NonExistentShare_ReturnsFailure()
    {
        var updateDto = new PlanShareUpdateDto(999, 1, 2, null, UserId, Permission.Edit, null);
        _planningService.Setup(s => s.UpdatePlanShareAsync(UserId, updateDto))
            .ReturnsAsync(Result<PlanShareDto>.Failure(PlanningErrors.PlanShareNotFound));

        var result = await _controller.UpdatePlanShare(updateDto);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.PlanShareNotFound.Code, result.Error.Code);
    }

    // --- DeletePlanShare ---

    [Fact]
    public async Task DeletePlanShare_ExistingShare_ReturnsSuccess()
    {
        _planningService.Setup(s => s.DeletePlanShareAsync(UserId, 1))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _controller.DeletePlanShare(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    [Fact]
    public async Task DeletePlanShare_NonExistentShare_ReturnsFailure()
    {
        _planningService.Setup(s => s.DeletePlanShareAsync(UserId, 999))
            .ReturnsAsync(Result<bool>.Failure(PlanningErrors.UnableToDelete));

        var result = await _controller.DeletePlanShare(999);

        Assert.False(result.IsSuccess);
        Assert.Equal(PlanningErrors.UnableToDelete.Code, result.Error.Code);
    }
}
