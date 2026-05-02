using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanService.Interfaces;
using PlanService.Models;
using Shared.Models;
using System.Security.Claims;


namespace PlanService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly IPlanningService _planningService;
    public PlansController(IPlanningService planningService)
    {
        _planningService = planningService;

    }
    //Todo: Get All plans for a user (both owned and shared[directly and in groups]) - need to add a new endpoint in the service and repository layers to support this
    [HttpGet("plans/user/{userId:int}")]
    [Authorize]
    public async Task<Result<IEnumerable<PlanDto>?>> GetAllUserPlans(int userId)
    {
        var result = await _planningService.GetPlansForUserAsync(userId);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanDto>?>.Success(result.Value);
    }

[HttpGet("plans/startdate/{startDate:datetime}")]
    [Authorize]
    public async Task<Result<IEnumerable<PlanDto>?>> GetPlansByStartDate(DateTime startDate)
    {
        var result = await _planningService.GetPlansByStartDateAsync(startDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanDto>?>.Success(result.Value);
    }

    [HttpGet("plans/enddate/{endDate:datetime}")]
    [Authorize]
    public async Task<Result<IEnumerable<PlanDto>?>> GetPlansByEndDate(DateTime endDate)
    {
        var result = await _planningService.GetPlansByEndDateAsync(endDate);
         if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanDto>?>.Success(result.Value);
    }
      

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<Result<PlanDto?>> GetById(int id)
    {
        var result = await _planningService.GetPlanByIdAsync(id);
        if (!result.IsSuccess)
        {
            return Result<PlanDto?>.Failure(result.Error);
        }
        return Result<PlanDto?>.Success(result.Value);
    }

        [HttpGet("plans/daterange")]
    [Authorize]
    public async Task<Result<IEnumerable<PlanDto>?>> GetPlansByDateRange(DateTime startDate, DateTime endDate)
    {
        var result = await _planningService.GetPlansByDateRangeAsync(startDate, endDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanDto>?>.Success(result.Value);
    }
    [HttpPost]
    [Authorize]
    public async Task<Result<PlanDto?>> CreatePlan(PlanCreateDto plan)
    {
        var result = await _planningService.CreatePlanAsync(plan);
        if (!result.IsSuccess)
        {
            return Result<PlanDto?>.Failure(result.Error);
        }
        return Result<PlanDto?>.Success(result.Value);
    }

    [HttpPut]
    [Authorize]
    public async Task<Result<PlanDto?>> UpdatePlan(PlanUpdateDto plan)
    {
        var result = await _planningService.UpdatePlanAsync(plan);
        if (!result.IsSuccess)
        {
            return Result<PlanDto?>.Failure(result.Error);
        }
        return Result<PlanDto?>.Success(result.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<Result<bool>> DeletePlan(int id)
    {
        var result = await _planningService.DeletePlanAsync(id);
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
        var result = await _planningService.CreatePlanShareAsync(planShare);
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
        var result = await _planningService.GetPlanSharesByPlanIdAsync(planId);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<PlanShareDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<PlanShareDto>?>.Success(result.Value);
    }
    [HttpGet("share/user/{userId:int}")]
    [Authorize]
    public async Task<Result<IEnumerable<PlanShareDto>?>> GetPlanSharesByUserId(int userId)
    {
        var result = await _planningService.GetPlanSharesByUserIdAsync(userId);
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
        var result = await _planningService.UpdatePlanShareAsync(planShare);
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
        var result = await _planningService.DeletePlanShareAsync(planShareId);
        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.Error);
        }
        return Result<bool>.Success(result.Value);
    }


}
