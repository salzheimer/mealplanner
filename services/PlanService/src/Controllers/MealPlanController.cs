using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using PlanService.Interfaces;
using PlanService.Models;


namespace PlanService.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]

public class MealPlanController : BaseController
{
    private readonly IMealPlanService _mealPlanService;

    public MealPlanController(IMealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    //Meal Plan Endpoints

    [HttpPost]
     
    public async Task<IActionResult> CreateMealPlan([FromBody] MealPlanCreateDto mealPlan)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<MealPlanDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.CreateMealPlanAsync(authenticatedUserId.Value, mealPlan));
        return result;
    }
    [HttpGet("user-meal-plans")]
    
    public async Task<IActionResult> GetAllUserMealPlans()
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<MealPlanDto>?>.Failure(MealPlanErrors.Unauthorized));
        }

        var result = HandleResult(await _mealPlanService.GetMealPlansForUserAsync(authenticatedUserId.Value));
        return result;
    }

    [HttpGet("{id:int}")]
     
    public async Task<IActionResult> GetMealPlanById(int id)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<MealPlanDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.GetMealPlanByIdAsync(authenticatedUserId.Value, id));
        return result;
    }


    [HttpGet("serve-date")]
     
    public async Task<IActionResult> GetMealPlansForDate([FromQuery] DateTime serveDate)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<MealPlanDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.GetMealPlansByStartDateAsync(authenticatedUserId.Value, serveDate));
        return result;
    }
    [HttpGet("end-date")]
     
    public async Task<IActionResult> GetMealPlansForEndDate([FromQuery] DateTime endDate)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<MealPlanDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.GetMealPlansByEndDateAsync(authenticatedUserId.Value, endDate));
        return result;
    }
    [HttpGet("date-range")]
    
    public async Task<IActionResult> GetMealPlansForDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<MealPlanDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.GetMealPlansByDateRangeAsync(authenticatedUserId.Value, startDate, endDate));
        return result;
    }

    [HttpPut]
    
    public async Task<IActionResult> UpdateMealPlan([FromBody] MealPlanUpdateDto mealPlan)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<MealPlanDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.UpdateMealPlanAsync(authenticatedUserId.Value, mealPlan));
        return result;
    }
    [HttpDelete("{id:int}")]
     
    public async Task<IActionResult> DeleteMealPlan(int id)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<bool>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.DeleteMealPlanAsync(authenticatedUserId.Value, id));
        return result;
    }

    // MealItemPlan Endpoints
    [HttpPost("{mealplanId:int}/mealitems/")]
    
    public async Task<IActionResult> AddMealItemToPlan(int mealPlanId, [FromBody] MealItemPlanCreateDto mealItemPlan)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<MealItemPlanDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.AddMealItemToPlanAsync(authenticatedUserId.Value, mealPlanId, mealItemPlan));
        return result;
    }
    [HttpGet("{mealplanId:int}/mealitems")]
     
    public async Task<IActionResult> GetMealItemsForPlan(int mealplanId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<IEnumerable<MealItemPlanDto>?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.GetMealItemsForMealPlanAsync(authenticatedUserId.Value, mealplanId));
        return result;
    }
    [HttpPut("{mealPlanId:int}/mealitems/{mealItemId}")]
    
    public async Task<IActionResult> UpdateMealItemInPlan(int mealPlanId, int mealItemId,[FromBody] MealItemPlanUpdateDto mealItemPlan)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<MealItemPlanDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.UpdateMealItemInPlanAsync(authenticatedUserId.Value,mealPlanId, mealItemId ,mealItemPlan));
        return result;
    }
    [HttpDelete("mealitem/{mealItemPlanId:int}")]
    
    public async Task<IActionResult> RemoveMealItemFromPlan(int mealItemPlanId)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<bool>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.RemoveMealItemFromPlanAsync(authenticatedUserId.Value, mealItemPlanId));
        return result;
    }



}