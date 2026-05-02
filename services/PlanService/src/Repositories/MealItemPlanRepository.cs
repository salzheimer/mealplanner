using PlanService.Models;
using PlanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PlanService.Repositories;

public class MealItemPlanRepository : IMealItemPlanRepository
{
    private readonly PlanContext _context;

    public MealItemPlanRepository(PlanContext context)
    {
        _context = context;
    }

    public async Task<MealItemPlan?> AddMealItemToMealPlanAsync(MealItemPlan mealItemPlan)
    {
        await _context.MealItemPlans.AddAsync(mealItemPlan);
        var result = await _context.SaveChangesAsync();
        if (result <= 0) return null!;
        return mealItemPlan;
    }

    public async Task<IEnumerable<MealItemPlan>> GetMealItemsForMealPlanAsync(int mealPlanId)
    {
        return await _context.MealItemPlans.Where(mip => mip.MealPlanId == mealPlanId).ToListAsync();
    }

    public async Task<bool> UpdateMealItemInMealPlanAsync(MealItemPlan mealItemPlan)
    {
        _context.MealItemPlans.Update(mealItemPlan);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> RemoveMealItemFromMealPlanAsync(int mealItemPlanId)
    {
        var mealItemPlan = await _context.MealItemPlans.FindAsync(mealItemPlanId);
        if (mealItemPlan != null)
        {
            _context.MealItemPlans.Remove(mealItemPlan);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        return false;
    }
}