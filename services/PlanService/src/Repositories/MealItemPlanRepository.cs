using PlanService.Models;
using PlanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PlanService.Repositories;

public class MealItemPlanRepository : IMealItemPlanRepository
{
    private readonly PlanDbContext _context;

    public MealItemPlanRepository(PlanDbContext context)
    {
        _context = context;
    }
private IQueryable<PlanMealItem> WithLookups() =>
        _context.MealItemPlans.Include(mi=>mi.MealItemPlanStatusType);
    public async Task<PlanMealItem?> AddMealItemToMealPlanAsync(PlanMealItem mealItemPlan)
    {
        await _context.MealItemPlans.AddAsync(mealItemPlan);
        var result = await _context.SaveChangesAsync();
        if (result <= 0) return null!;
        return mealItemPlan;
    }

    public async Task<PlanMealItem?> GetByIdAsync(Guid mealItemPlanId)
    {
        return await WithLookups().FirstOrDefaultAsync(mip => mip.Id == mealItemPlanId);
    }

    public async Task<IEnumerable<PlanMealItem>> GetMealItemsForMealPlanAsync(Guid mealPlanId)
    {
        return await WithLookups().Where(mip => mip.MealPlanId == mealPlanId).ToListAsync();
    }

    public async Task<bool> UpdateMealItemInMealPlanAsync(PlanMealItem mealItemPlan)
    {
        _context.MealItemPlans.Update(mealItemPlan);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> RemoveMealItemFromMealPlanAsync(Guid mealItemPlanId)
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

public class MealItemPlanStatusTypeRepository : Interfaces.IPlanMealItemStatusTypeRepository
{
    private readonly PlanDbContext _context; 
    public MealItemPlanStatusTypeRepository(PlanDbContext context)
    {
        _context = context;
    }   
    public async Task<MealItemPlanStatusType?> GetByIdAsync(int id)
    {
        return await _context.MealItemPlanStatusTypes.FindAsync(id);
    }
    public async Task<MealItemPlanStatusType?> GetByNameAsync(string name)
    {
        return await _context.MealItemPlanStatusTypes.FirstOrDefaultAsync(rt => rt.Name == name);
    }
    public async Task<IEnumerable<MealItemPlanStatusType>> GetAllAsync()
    {
        return await _context.MealItemPlanStatusTypes.ToListAsync();
    }
    public async Task<MealItemPlanStatusType?> CreateAsync(MealItemPlanStatusType statusType)
    {
        var entry = await _context.MealItemPlanStatusTypes.AddAsync(statusType);
        await _context.SaveChangesAsync();
        return entry.Entity;  
    }
    public async Task<bool> UpdateAsync(MealItemPlanStatusType statusType)
    {
        var existing = await _context.MealItemPlanStatusTypes.FindAsync(statusType.Id);
        if (existing == null) return false;

        existing.Name = statusType.Name;
        existing.DisplayName = statusType.DisplayName;
        existing.SortOrder = statusType.SortOrder;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var resourceType = await _context.MealItemPlanStatusTypes.FindAsync(id);
        if (resourceType == null) return false;   

        _context.MealItemPlanStatusTypes.Remove(resourceType);
        var result = await _context.SaveChangesAsync();
        return result > 0;  
    }
}