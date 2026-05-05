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
    public async Task<Result<IEnumerable<PlanSummaryDto>?>> GetAllUserPlans()
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {            return Result<IEnumerable<PlanSummaryDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.GetPlansForUserAsync(authenticatedUserId.Value);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanSummaryDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanSummaryDto>?>.Success(result.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<Result<IEnumerable<PlanSummaryDto>?>> GetPlansByStartDate([FromQuery]DateTime startDate)
    {   var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)        {
            return Result<IEnumerable<PlanSummaryDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.GetPlansByStartDateAsync(authenticatedUserId.Value, startDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanSummaryDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanSummaryDto>?>.Success(result.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<Result<IEnumerable<PlanSummaryDto>?>> GetPlansByEndDate([FromQuery]DateTime endDate)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<IEnumerable<PlanSummaryDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.GetPlansByEndDateAsync(userId.Value, endDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanSummaryDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanSummaryDto>?>.Success(result.Value);
    }


    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<Result<PlanSummaryDto?>> GetById(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<PlanSummaryDto?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.GetPlanByIdAsync(id);
        if (!result.IsSuccess)
        {
            return Result<PlanSummaryDto?>.Failure(result.Error);
        }
        return Result<PlanSummaryDto?>.Success(result.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<Result<IEnumerable<PlanSummaryDto>?>> GetPlansByDateRange([FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<IEnumerable<PlanSummaryDto>?>.Failure(MealPlanErrors.Unauthorized);
        }       
        var result = await _planningService.GetPlansByDateRangeAsync(userId.Value, startDate, endDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanSummaryDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanSummaryDto>?>.Success(result.Value);
    }
    [HttpPost]
    [Authorize]
    public async Task<Result<PlanSummaryDto?>> CreatePlan(PlanCreateDto plan)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<PlanSummaryDto?>.Failure(MealPlanErrors.Unauthorized);
        }
        
        var result = await _planningService.CreatePlanAsync(userId.Value, plan);
        if (!result.IsSuccess)
        {
            return Result<PlanSummaryDto?>.Failure(result.Error);
        }
        return Result<PlanSummaryDto?>.Success(result.Value);
    }

    [HttpPut]
    [Authorize]
    public async Task<Result<PlanSummaryDto?>> UpdatePlan(PlanUpdateDto plan)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<PlanSummaryDto?>.Failure(MealPlanErrors.Unauthorized);
        }   
        var result = await _planningService.UpdatePlanAsync(userId.Value, plan);
        if (!result.IsSuccess)
        {
            return Result<PlanSummaryDto?>.Failure(result.Error);
        }
        return Result<PlanSummaryDto?>.Success(result.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<Result<bool>> DeletePlan(int id)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<bool>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.DeletePlanAsync(userId.Value, id);
        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.Error);
        }
        return Result<bool>.Success(result.Value);
    }

    [HttpPost("share")]
    [Authorize]
    public async Task<Result<PlanShareDto?>> CreatePlanShare(PlanShareCreateDto planShare)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<PlanShareDto?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.CreatePlanShareAsync(userId.Value, planShare);
        if (!result.IsSuccess)
        {
            return Result<PlanShareDto?>.Failure(result.Error);
        }
        return Result<PlanShareDto?>.Success(result.Value);
    }
    [HttpGet("share/plan/{planId:int}")]
    [Authorize]
    public async Task<Result<IEnumerable<PlanShareDto>?>> GetPlanSharesByPlanId(int planId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<IEnumerable<PlanShareDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.GetPlanSharesByPlanIdAsync(userId.Value, planId);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanShareDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanShareDto>?>.Success(result.Value);
    }
    [HttpGet("user-shared-plans")]
    [Authorize]
    public async Task<Result<IEnumerable<PlanShareDto>?>> GetPlanSharesByUserId()
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return Result<IEnumerable<PlanShareDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.GetPlanSharesBySharedByUserIdAsync(userId.Value);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanShareDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanShareDto>?>.Success(result.Value);
    }
    [HttpPut("share")]
    [Authorize]
    public async Task<Result<PlanShareDto?>> UpdatePlanShare(PlanShareUpdateDto planShare)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<PlanShareDto?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.UpdatePlanShareAsync(userId.Value, planShare);
        if (!result.IsSuccess)
        {
            return Result<PlanShareDto?>.Failure(result.Error);
        }
        return Result<PlanShareDto?>.Success(result.Value);
    }

    [HttpDelete("share/{planShareId:int}")]
    [Authorize]
    public async Task<Result<bool>> DeletePlanShare(int planShareId)
    {
        var userId = GetAuthenticatedUserId();
        if (userId == null)        {
            return Result<bool>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _planningService.DeletePlanShareAsync(userId.Value, planShareId);
        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.Error);
        }
        return Result<bool>.Success(result.Value);
    }


}
