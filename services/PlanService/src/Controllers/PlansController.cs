using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanService.Interfaces;
using PlanService.Models;
using Shared.Models;


namespace PlanService.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PlansController : BaseController
{
    private readonly IPlanningService _planningService;
    public PlansController(IPlanningService planningService)
    {
        _planningService = planningService;

    }
    [HttpGet()]

    public async Task<IActionResult> GetAllUserPlans()
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<PlanSummaryResponse>?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansForUserAsync(authenticatedUserId.Value));
        return result;
    }

    [HttpGet("start-date")]

    public async Task<IActionResult> GetPlansByStartDate([FromQuery] DateTime startDate)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<PlanSummaryResponse>?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansByStartDateAsync(authenticatedUserId.Value, startDate));
        return result;
    }

    [HttpGet("end-date")]

    public async Task<IActionResult> GetPlansByEndDate([FromQuery] DateTime endDate)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<PlanSummaryResponse>?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansByEndDateAsync(userId.Value, endDate));
        return result;
    }


    [HttpGet("{id:Guid}")]

    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryResponse?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlanByIdAsync(userId.Value, id));
        return result;

    }

    [HttpGet("date-range")]

    public async Task<IActionResult> GetPlansByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<PlanSummaryResponse>?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansByDateRangeAsync(userId.Value, startDate, endDate));
        return result;
    }

    [HttpGet("shared-with-me")]
    public async Task<IActionResult> PlansSharedWithMe()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<PlanSummaryResponse>.Failure(PlanErrors.Unauthorized));

        return HandleResult(await _planningService.GetPlansSharedWithMeAsync(userId.Value));
    }
    [HttpPost]

    public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequest plan)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryResponse?>.Failure(PlanErrors.Unauthorized));
        }

        var result = HandleResult(await _planningService.CreatePlanAsync(userId.Value, plan));
        return result;
    }

    [HttpPut("{planId:Guid}")]

    public async Task<IActionResult> UpdatePlan(Guid planId, [FromBody] UpdatePlanRequest plan)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryResponse?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.UpdatePlanAsync(userId.Value,planId, plan));
        return result;
    }

    [HttpDelete("{id:Guid}")]

    public async Task<IActionResult> DeletePlan(Guid id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<bool>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.DeletePlanAsync(userId.Value, id));
        return result;
    }

    [HttpPost("shares")]

    public async Task<IActionResult> SharePlan([FromBody] SharePlanRequest planShare)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<SharePlanResponse?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.SharePlanAsync(userId.Value, planShare));
        return result;
    }
   

   


}
