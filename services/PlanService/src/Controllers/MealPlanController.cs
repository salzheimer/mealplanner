using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using PlanService.Interfaces;
using PlanService.Models;
using System.Reflection.Metadata;

namespace PlanService.Controllers;

[ApiController]
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
    [Authorize]
    public async Task<IActionResult> CreateMealPlan(MealPlanCreateDto mealPlan)
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
    [Authorize]
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
    [Authorize]
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


    [HttpGet]
    [Authorize]
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
    [HttpGet]
    [Authorize]
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
    [HttpGet]
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> UpdateMealPlan(MealPlanUpdateDto mealPlan)
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
    [Authorize]
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
    [HttpPost("mealitem")]
    [Authorize]
    public async Task<IActionResult> AddMealItemToPlan(MealItemPlanCreateDto mealItemPlan)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<MealItemPlanDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.AddMealItemToPlanAsync(authenticatedUserId.Value, mealItemPlan));
        return result;
    }
    [HttpGet("mealitems/{mealplanId:int}")]
    [Authorize]
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
    [HttpPut("mealitem")]
    [Authorize]
    public async Task<IActionResult> UpdateMealItemInPlan(MealItemPlanUpdateDto mealItemPlan)
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return HandleResult(Result<MealItemPlanDto?>.Failure(MealPlanErrors.Unauthorized));
        }
        var result = HandleResult(await _mealPlanService.UpdateMealItemInPlanAsync(authenticatedUserId.Value, mealItemPlan));
        return result;
    }
    [HttpDelete("mealitem/{mealItemPlanId:int}")]
    [Authorize]
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