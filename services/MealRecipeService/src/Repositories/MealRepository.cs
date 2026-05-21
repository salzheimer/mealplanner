using MealRecipeService.Models;
using MealRecipeService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;

public class MealRepository : Interfaces.IMealRepository
{
    private readonly MealDbContext _context;

    public MealRepository(MealDbContext context)
    {
        _context = context;
    }

    public async Task<Meal?> GetByIdAsync(int id)
    {
        return await _context.Meals.FindAsync(id);
    }

    public async Task<IEnumerable<Meal>> GetByOwnerIdAsync(int userId)
    {
        return await _context.Meals.Where(m => m.OwnerUserId == userId).ToListAsync();
    }
    public async Task<IEnumerable<Meal>> GetByIdsAsync(HashSet<int> sharedMealIds)
    {
        return await _context.Meals.Where(r => sharedMealIds.Contains(r.Id)).ToListAsync();
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

    public async Task<bool> DeleteAsync(int id)
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
