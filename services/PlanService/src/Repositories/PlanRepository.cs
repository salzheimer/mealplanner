using PlanService.Models;
using PlanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PlanService.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly PlanDbContext _context;

    public PlanRepository(PlanDbContext context)
    {
        _context = context;
    }

    
    public async Task<Plan?> GetPlanByIdAsync(Guid id)
    {
        return await _context.Plans.FirstOrDefaultAsync(p => p.PlanId == id);
    }
    

    public async Task<bool> UpdatePlanAsync(Plan plan)
    {
        _context.Plans.Update(plan);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeletePlanAsync(Guid id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan != null)
        {
            _context.Plans.Remove(plan);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        return false;
    }
    

    public async Task<Plan?> CreatePlanAsync(Plan plan)
    {
         await _context.Plans.AddAsync(plan);
         var result = await _context.SaveChangesAsync();
            if (result <= 0) return null!;
         return plan;
    }

    public async Task<IEnumerable<Plan>> GetPlansByOwnerAsync(Guid userId)
    {
        var plans = await _context.Plans.Where(p => p.OwnerUserId == userId).ToListAsync();

        return plans;
    }

    public async Task<IEnumerable<Plan>> GetPlansByStartDateAsync(DateTime startDate)
    {
        return await _context.Plans.Where(p => p.StartDate >= startDate).ToListAsync();
    }
     

    public async Task<IEnumerable<Plan>> GetPlansByEndDateAsync(DateTime endDate)
    {
        return await _context.Plans.Where(p => p.EndDate <= endDate).ToListAsync();
    }

    public async Task<IEnumerable<Plan>> GetPlansByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Plans.Where(p => p.StartDate >= startDate && p.EndDate <= endDate).ToListAsync();
    }

      public async Task<IEnumerable<Plan>> GetByIdsAsync(HashSet<Guid> sharedPlanIds)
    {
        return await _context.Plans.Where(p => sharedPlanIds.Contains(p.PlanId)).ToListAsync();
    }
}