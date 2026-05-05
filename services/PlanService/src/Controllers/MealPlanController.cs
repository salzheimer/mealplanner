using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using PlanService.Interfaces;
using PlanService.Models;

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
    public async Task<Result<MealPlanDto?>> CreateMealPlan(MealPlanCreateDto mealPlan)
    {var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)        {
            return Result<MealPlanDto?>.Failure(MealPlanErrors.Unauthorized);
        }       
        var result = await _mealPlanService.CreateMealPlanAsync(authenticatedUserId.Value, mealPlan);
        if (!result.IsSuccess)
        {
            return Result<MealPlanDto?>.Failure(result.Error);
        }

        return Result<MealPlanDto?>.Success(result.Value);
    }
    [HttpGet("user-meal-plans")]
    [Authorize]
    public async Task<Result<IEnumerable<MealPlanDto>?>> GetAllUserMealPlans()
    {
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
    
        var result = await _mealPlanService.GetMealPlansForUserAsync(authenticatedUserId.Value);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<MealPlanDto>?>.Success(result.Value);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<Result<MealPlanDto?>> GetMealPlanById(int id)
    { var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<MealPlanDto?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.GetMealPlanByIdAsync(authenticatedUserId.Value, id);
        if (!result.IsSuccess)
        {
            return Result<MealPlanDto?>.Failure(result.Error);
        }
        return Result<MealPlanDto?>.Success(result.Value);
    }


    [HttpGet]
    [Authorize]
    public async Task<Result<IEnumerable<MealPlanDto>?>> GetMealPlansForDate([FromQuery]DateTime serveDate)
    {
         var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.GetMealPlansByStartDateAsync(authenticatedUserId.Value, serveDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<MealPlanDto>?>.Success(result.Value);
    }
    [HttpGet]
    [Authorize]
    public async Task<Result<IEnumerable<MealPlanDto>?  >> GetMealPlansForEndDate([FromQuery]DateTime endDate)
    { var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.GetMealPlansByEndDateAsync(authenticatedUserId.Value, endDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<MealPlanDto>?>.Success(result.Value);
    }
    [HttpGet]
    [Authorize]
    public async Task<Result<IEnumerable<MealPlanDto>?>> GetMealPlansForDateRange([FromQuery]DateTime startDate, [FromQuery]DateTime endDate)
    { var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.GetMealPlansByDateRangeAsync(authenticatedUserId.Value, startDate, endDate);
        if (!result.IsSuccess)
        {
            return Result<IEnumerable<MealPlanDto>?>.Failure(result.Error);
        }
        return Result<IEnumerable<MealPlanDto>?>.Success(result.Value);
    }

    [HttpPut]
    [Authorize]
    public async Task<Result<MealPlanDto?>> UpdateMealPlan(MealPlanUpdateDto mealPlan)
    { var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)
        {
            return Result<MealPlanDto?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.UpdateMealPlanAsync(authenticatedUserId.Value, mealPlan);
        if (!result.IsSuccess)
        {
            return Result<MealPlanDto?>.Failure(result.Error);

        }
        return Result<MealPlanDto?>.Success(result.Value);
    }
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<Result<bool>> DeleteMealPlan(int id)
    {   var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)        {
            return Result<bool>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.DeleteMealPlanAsync(authenticatedUserId.Value, id);
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
        var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)        {
            return Result<MealItemPlanDto?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.AddMealItemToPlanAsync(authenticatedUserId.Value, mealItemPlan);
        if (!result.IsSuccess)        {
            return Result<MealItemPlanDto?>.Failure(result.Error);  
        }
        return Result<MealItemPlanDto?>.Success(result.Value);
    }
    [HttpGet("mealitems/{mealplanId:int}")]
    [Authorize]
    public async Task<Result<IEnumerable<MealItemPlanDto>?>> GetMealItemsForPlan(int mealplanId)
    {
            var authenticatedUserId = GetAuthenticatedUserId();  
        if (authenticatedUserId  == null)        {
            return Result<IEnumerable<MealItemPlanDto>?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.GetMealItemsForMealPlanAsync(authenticatedUserId.Value, mealplanId);
        if (!result.IsSuccess)        {
            return Result<IEnumerable<MealItemPlanDto>?>.Failure(result.Error);      
        }
        return Result<IEnumerable<MealItemPlanDto>?>.Success(result.Value);

    }
    [HttpPut("mealitem")]
    [Authorize]
    public async Task<Result<MealItemPlanDto?>> UpdateMealItemInPlan(MealItemPlanUpdateDto mealItemPlan)
    {
            var authenticatedUserId = GetAuthenticatedUserId();  
        if (authenticatedUserId == null)        {
            return Result<MealItemPlanDto?>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.UpdateMealItemInPlanAsync(authenticatedUserId.Value, mealItemPlan);
        if (!result.IsSuccess)        {
            return Result<MealItemPlanDto?>.Failure(result.Error);   
        }
        return Result<MealItemPlanDto?>.Success(result.Value);

    }
    [HttpDelete("mealitem/{mealItemPlanId:int}")]
    [Authorize]
    public async Task<Result<bool>> RemoveMealItemFromPlan(int mealItemPlanId)
    {
            var authenticatedUserId = GetAuthenticatedUserId();
        if (authenticatedUserId == null)        {
            return Result<bool>.Failure(MealPlanErrors.Unauthorized);
        }
        var result = await _mealPlanService.RemoveMealItemFromPlanAsync(authenticatedUserId.Value, mealItemPlanId);
        if (!result.IsSuccess)        {
            return Result<bool>.Failure(result.Error);  
        }
        return Result<bool>.Success(result.Value);
    }



}