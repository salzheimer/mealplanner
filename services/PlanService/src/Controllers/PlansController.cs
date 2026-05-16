using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanService.Interfaces;
using PlanService.Models;
using Shared.Models;
using System.Security.Claims;


namespace PlanService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : BaseController
{
    private readonly IPlanningService _planningService;
    public PlansController(IPlanningService planningService)
    {
        _planningService = planningService;

    }
    //Todo: Get All plans for a user (both owned and shared[directly and in groups]) - need to add a new endpoint in the service and repository layers to support this
    [HttpGet("user-plans")]
    [Authorize]
    public async Task<IActionResult> GetAllUserPlans()
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<PlanSummaryDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansForUserAsync(authenticatedUserId.Value));
        return result;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPlansByStartDate([FromQuery] DateTime startDate)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<PlanSummaryDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansByStartDateAsync(authenticatedUserId.Value, startDate));
        return result;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPlansByEndDate([FromQuery] DateTime endDate)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<PlanSummaryDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansByEndDateAsync(userId.Value, endDate));
        return result;
    }


    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlanByIdAsync(userId.Value, id));
        return result;

    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPlansByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<PlanSummaryDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansByDateRangeAsync(userId.Value, startDate, endDate));
        return result;
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePlan(PlanCreateDto plan)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryDto?>.Failure(MealPlanErrors.Unauthorized));
        }

        var result = HandleResult(await _planningService.CreatePlanAsync(userId.Value, plan));
        return result;
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdatePlan(PlanUpdateDto plan)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.UpdatePlanAsync(userId.Value, plan));
        return result;
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeletePlan(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<bool>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.DeletePlanAsync(userId.Value, id));
        return result;
    }

    [HttpPost("share")]
    [Authorize]
    public async Task<IActionResult> CreatePlanShare(PlanShareCreateDto planShare)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanShareDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.CreatePlanShareAsync(userId.Value, planShare));
        return result;
    }
    [HttpGet("share/plan/{planId:int}")]
    [Authorize]
    public async Task<IActionResult> GetPlanSharesByPlanId(int planId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<PlanShareDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlanSharesByPlanIdAsync(userId.Value, planId));
        return result;
    }
    [HttpGet("user-shared-plans")]
    [Authorize]
    public async Task<IActionResult> GetPlanSharesByUserId()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<PlanShareDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlanSharesBySharedByUserIdAsync(userId.Value));
        return result;
    }
    [HttpPut("plan-share")]
    [Authorize]
    public async Task<IActionResult> UpdatePlanShare(PlanShareUpdateDto planShare)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanShareDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.UpdatePlanShareAsync(userId.Value, planShare));
        return result;
    }

    [HttpDelete("plan-share/{planShareId:int}")]
    [Authorize]
    public async Task<IActionResult> DeletePlanShare(int planShareId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<bool>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.DeletePlanShareAsync(userId.Value, planShareId));
        return result;
    }


}
