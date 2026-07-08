using PlanService.Models;
using PlanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PlanService.Repositories;

public class MealPlanRepository : IMealPlanRepository
{
    private readonly PlanDbContext _context;

    public MealPlanRepository(PlanDbContext context)
    {
        _context = context;
    }
    public async Task<PlanMeal> CreateMealPlanAsync(PlanMeal mealPlan)
    {
        await _context.MealPlans.AddAsync(mealPlan);
        var result = await _context.SaveChangesAsync();
        if (result <= 0) return null!;
        return mealPlan;
    }
    public async Task<PlanMeal?> GetMealPlanByIdAsync(Guid id)
    {
        return await _context.MealPlans.FindAsync(id);
    }
    public async Task<IEnumerable<PlanMeal?>> GetMealPlansByPlanIdAsync(Guid planId)
    {
        return await _context.MealPlans.Where(mp => mp.PlanId == planId).ToListAsync();
    }

    public async Task<IEnumerable<PlanMeal>> GetMealPlansForUserAsync(Guid userId)
    {
        var mealPlans = await _context.MealPlans

            .Where(mp => mp.AddedByUserId == userId)
            .ToListAsync();

        return mealPlans;
    }

    public async Task<IEnumerable<PlanMeal>> GetMealPlansByStartDateAsync(DateTime startDate)
    {
        return await _context.MealPlans.Where(mp => mp.ServeDate >= startDate).ToListAsync();
    }

    public async Task<IEnumerable<PlanMeal>> GetMealPlansByEndDateAsync(DateTime endDate)
    {
        return await _context.MealPlans.Where(mp => mp.EndDate <= endDate).ToListAsync();
    }

    public async Task<IEnumerable<PlanMeal>> GetMealPlansByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.MealPlans.Where(mp => mp.ServeDate >= startDate && mp.EndDate <= endDate).ToListAsync();
    }
    public async Task<bool> UpdateMealPlanAsync(PlanMeal mealPlan)
    {
        _context.MealPlans.Update(mealPlan);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteMealPlanAsync(Guid id)
    {
        var mealPlans = await _context.MealPlans.Where(mp => mp.Id == id).ToListAsync();
        if (mealPlans.Any())
        {
            _context.MealPlans.RemoveRange(mealPlans);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        return false;
    }


}