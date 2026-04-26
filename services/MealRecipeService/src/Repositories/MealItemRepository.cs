using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;

public class MealItemRepository : Interfaces.IMealItemRepository
{
    private readonly MealDbContext _context;

    public MealItemRepository(MealDbContext context)
    {
        _context = context;
    }

    public async Task<MealItem?> GetByIdAsync(int id)
    {
        return await _context.MealItems.FindAsync(id);
    }

    public async Task<IEnumerable<MealItem>> GetByMealIdAsync(int mealId)
    {
        return await _context.MealItems.Where(mi => mi.MealId == mealId).ToListAsync();
    }

    public async Task<MealItem?> CreateAsync(MealItem mealItem)
    {
        _context.MealItems.Add(mealItem);
       var result = await _context.SaveChangesAsync();
        if (result <= 0) return null!;
        return mealItem;
    }

    public async Task<bool> UpdateAsync(MealItem mealItem)
    {
        _context.Entry(mealItem).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var mealItem = await _context.MealItems.FindAsync(id);
        if (mealItem == null)
        {
            return false;
        }
        _context.MealItems.Remove(mealItem);
        var result = await _context.SaveChangesAsync();
        return result > 0;

    }
}