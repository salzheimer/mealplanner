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
            return HandleResult(Result<IEnumerable<PlanSummaryDto>?>.Failure(PlanErrors.Unauthorized));
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
            return HandleResult(Result<IEnumerable<PlanSummaryDto>?>.Failure(PlanErrors.Unauthorized));
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
            return HandleResult(Result<IEnumerable<PlanSummaryDto>?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansByEndDateAsync(userId.Value, endDate));
        return result;
    }


    [HttpGet("{id:int}")]

    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryDto?>.Failure(PlanErrors.Unauthorized));
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
            return HandleResult(Result<IEnumerable<PlanSummaryDto>?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlansByDateRangeAsync(userId.Value, startDate, endDate));
        return result;
    }

    [HttpGet("shared-with-me")]
    public async Task<IActionResult> PlansSharedWithMe()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
            return HandleResult(Result<PlanSummaryDto>.Failure(PlanErrors.Unauthorized));

        return HandleResult(await _planningService.GetPlansSharedWithMeAsync(userId.Value));
    }
    [HttpPost]

    public async Task<IActionResult> CreatePlan([FromBody] PlanCreateDto plan)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryDto?>.Failure(PlanErrors.Unauthorized));
        }

        var result = HandleResult(await _planningService.CreatePlanAsync(userId.Value, plan));
        return result;
    }

    [HttpPut("{planId:int}")]

    public async Task<IActionResult> UpdatePlan(int planId, [FromBody] PlanUpdateDto plan)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanSummaryDto?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.UpdatePlanAsync(userId.Value,planId, plan));
        return result;
    }

    [HttpDelete("{id:int}")]

    public async Task<IActionResult> DeletePlan(int id)
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

    public async Task<IActionResult> CreatePlanShare([FromBody] PlanShareCreateDto planShare)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanShareDto?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.CreatePlanShareAsync(userId.Value, planShare));
        return result;
    }
    [HttpGet("{planId:int}/shares")]

    public async Task<IActionResult> GetPlanSharesByPlanId(int planId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<PlanShareDto>?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlanSharesByPlanIdAsync(userId.Value, planId));
        return result;
    }
    [HttpGet("plans-shared-by-user")]

    public async Task<IActionResult> GetPlanSharesByUserId()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<IEnumerable<PlanShareDto>?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.GetPlanSharesBySharedByUserIdAsync(userId.Value));
        return result;
    }
    [HttpPut("{planId:int}/shares/{shareId:int}")]

    public async Task<IActionResult> UpdatePlanShare(int shareId,[FromBody] PlanShareUpdateDto planShare)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<PlanShareDto?>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.UpdatePlanShareAsync(userId.Value, shareId,planShare));
        return result;
    }

    [HttpDelete("{planId:int}/shares/{planShareId:int}")]

    public async Task<IActionResult> DeletePlanShare(int planId, int planShareId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return HandleResult(Result<bool>.Failure(PlanErrors.Unauthorized));
        }
        var result = HandleResult(await _planningService.DeletePlanShareAsync(userId.Value, planShareId));
        return result;
    }


}
