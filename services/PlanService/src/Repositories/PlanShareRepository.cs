using PlanService.Models;
using PlanService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PlanService.Repositories;

public class PlanShareRepository : IPlanShareRepository
{
    private readonly PlanContext _context;

    public PlanShareRepository(PlanContext context)
    {
        _context = context;
    }

    public async Task<PlanShare> CreatePlanShareAsync(PlanShare planShare)
    {
        _context.PlanShares.Add(planShare);
        await _context.SaveChangesAsync();
        return planShare;
    }

    public async Task<bool> DeletePlanShareAsync(int planShareId)
    {
        var planShare = await _context.PlanShares.FindAsync(planShareId);
        if (planShare == null) return false;

        _context.PlanShares.Remove(planShare);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PlanShare>> GetPlanSharesByPlanIdAsync(int planId)
    {
        return await _context.PlanShares.Where(ps => ps.PlanId == planId).ToListAsync();
    }
    public async Task<PlanShare?> GetPlanShareByIdAsync(int planShareId)
    {
        return await _context.PlanShares.FindAsync(planShareId);
    }

    public async Task<IEnumerable<PlanShare>> GetPlanSharesBySharedByUserIdAsync(int userId)
    {
        return await _context.PlanShares.Where(ps => ps.SharedByUserId == userId).ToListAsync();
    }

    public async Task<bool> UpdatePlanShareAsync(PlanShare planShare)
    {
        _context.PlanShares.Update(planShare);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}