using PlanService.Models;
using Microsoft.EntityFrameworkCore;

namespace PlanService.Repositories;

public class PlanRepository : IPlanRepository
{
    private readonly PlanContext _context;

    public PlanRepository(PlanContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Plan>> GetAllGroupPlansAsync(int groupId)
    {
        return await _context.Plans.Where(p => p.GroupId == groupId).ToListAsync();
    }
    public async Task<Plan?> GetPlanByIdAsync(int id)
    {
        return await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task AddPlanAsync(Plan plan)
    {
        await _context.Plans.AddAsync(plan);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePlanAsync(Plan plan)
    {
        _context.Plans.Update(plan);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePlanAsync(int id)
    {
        var plan = await _context.Plans.FindAsync(id);
        if (plan != null)
        {
            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
        }
    }



}