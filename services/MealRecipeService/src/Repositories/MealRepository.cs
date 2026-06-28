using MealRecipeService.Models;
using MealRecipeService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;

public class MealRepository : Interfaces.IMealRepository
{
    private readonly MealRecipeDbContext _context;

    public MealRepository(MealRecipeDbContext context)
    {
        _context = context;
    }

    private IQueryable<Meal> WithLookups() =>
        _context.Meals.Include(m => m.MealType);

    public async Task<Meal?> GetByIdAsync(Guid id)
    {
        return await WithLookups().FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Meal>> GetByOwnerIdAsync(Guid userId)
    {
        return await WithLookups().Where(m => m.OwnerUserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Meal>> GetByIdsAsync(HashSet<Guid> sharedMealIds)
    {
        return await WithLookups().Where(m => sharedMealIds.Contains(m.Id)).ToListAsync();
    }
    public async Task<Meal?> CreateAsync(Meal meal)
    {
        _context.Meals.Add(meal);
        var result = await _context.SaveChangesAsync();
        if (result <= 0) return null!;
        return meal;
    }

    public async Task<bool> UpdateAsync(Meal meal)
    {
        _context.Entry(meal).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var meal = await _context.Meals.FindAsync(id);
        if (meal == null)
        {
            return false;
        }
        _context.Meals.Remove(meal);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }


}
