using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using PlanService.Interfaces;

namespace PlanService.Controllers;

[ApiController]
[Route("api/[controller]")]

public class MealPlanController : ControllerBase
{
    private readonly IMealPlanService _mealPlanService;

    public MealPlanController(IMealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    //Meal Plan Endpoints

    [HttpPost]
    [Authorize]
    public async Task<Result<MealPlanDto?>> CreateMealPlan(MealPlanCreateDto mealPlan)
    {
        var result = await _mealPlanService.CreateMealPlanAsync(mealPlan);
        if (!result.IsSuccess)
        {
            return Result<MealPlanDto?>.Failure(result.Error);
        }

        return Result<MealPlanDto?>.Success(result.Value);
    }
    [HttpGet("user-plans/{userId:int}")]
    [Authorize]
    public async Task<Result<IEnumerable<MealPlanDto>?>> GetAllUserMealPlans(int userId)
    {
        var result = await _mealPlanService.GetMealPlansForUserAsync(userId);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<MealPlanDto>?>.Success(result.Value);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<Result<MealPlanDto?>> GetMealPlanById(int id)
    {
        var result = await _mealPlanService.GetMealPlanByIdAsync(id);
        if (!result.IsSuccess)
        {
            return Result<MealPlanDto?>.Failure(result.Error);
        }
        return Result<MealPlanDto?>.Success(result.Value);
    }


    [HttpGet("/mealplan/servedate/{serveDate:datetime}")]
    [Authorize]
    public async Task<Result<IEnumerable<MealPlanDto>?>> GetMealPlansForDate(DateTime serveDate)
    {
        var result = await _mealPlanService.GetMealPlansByStartDateAsync(serveDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<MealPlanDto>?>.Success(result.Value);
    }
    [HttpGet("/mealplan/enddate/{endDate:datetime}")]
    [Authorize]
    public async Task<Result<IEnumerable<MealPlanDto>?  >> GetMealPlansForEndDate(DateTime endDate)
    {
        var result = await _mealPlanService.GetMealPlansByEndDateAsync(endDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<MealPlanDto>?>.Success(result.Value);
    }
    [HttpGet("{startDate:DateTime}/{endDate:DateTime}")]
    [Authorize]
    public async Task<Result<IEnumerable<MealPlanDto>?>> GetMealPlansForDateRange(DateTime startDate, DateTime endDate)
    {
        var result = await _mealPlanService.GetMealPlansByDateRangeAsync(startDate, endDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<MealPlanDto>?>.Success(result.Value);
    }

    [HttpPut]
    [Authorize]
    public async Task<Result<MealPlanDto?>> UpdateMealPlan(MealPlanUpdateDto mealPlan)
    {
        var result = await _mealPlanService.UpdateMealPlanAsync(mealPlan);
        if (!result.IsSuccess)
        {
            return Result<MealPlanDto?>.Failure(result.Error);

        }
        return Result<MealPlanDto?>.Success(result.Value);
    }
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<Result<bool>> DeleteMealPlan(int id)
    {
        var result = await _mealPlanService.DeleteMealPlanAsync(id);
        if (!result.IsSuccess)        {
            return Result<bool>.Failure(result.Error);  
        }
        return Result<bool>.Success(result.Value);
    }

    // MealItemPlan Endpoints
    [HttpPost("mealitem")]
    [Authorize]
    public async Task<Result<MealItemPlanDto?>> AddMealItemToPlan(MealItemPlanCreateDto mealItemPlan)
    {
        var result = await _mealPlanService.AddMealItemToPlanAsync(mealItemPlan);
        if (!result.IsSuccess)        {
            return Result<MealItemPlanDto?>.Failure(result.Error);  
        }
        return Result<MealItemPlanDto?>.Success(result.Value);
    }
    [HttpGet("mealitems/{mealplanId:int}")]
    [Authorize]
    public async Task<Result<IEnumerable<MealItemPlanDto>?>> GetMealItemsForPlan(int mealplanId)
    {
        var result = await _mealPlanService.GetMealItemsForMealPlanAsync(mealplanId);
        if (!result.IsSuccess)        {
            return Result<IEnumerable<MealItemPlanDto>?>.Failure(result.Error);      
        }
        return Result<IEnumerable<MealItemPlanDto>?>.Success(result.Value);

    }
    [HttpPut("mealitem")]
    [Authorize]
    public async Task<Result<MealItemPlanDto?>> UpdateMealItemInPlan(MealItemPlanUpdateDto mealItemPlan)
    {
        var result = await _mealPlanService.UpdateMealItemInPlanAsync(mealItemPlan);
        if (!result.IsSuccess)        {
            return Result<MealItemPlanDto?>.Failure(result.Error);   
        }
        return Result<MealItemPlanDto?>.Success(result.Value);

    }
    [HttpDelete("mealitem/{mealItemPlanId:int}")]
    [Authorize]
    public async Task<Result<bool>> RemoveMealItemFromPlan(int mealItemPlanId)
    {
        var result = await _mealPlanService.RemoveMealItemFromPlanAsync(mealItemPlanId);
        if (!result.IsSuccess)        {
            return Result<bool>.Failure(result.Error);  
        }
        return Result<bool>.Success(result.Value);
    }



}