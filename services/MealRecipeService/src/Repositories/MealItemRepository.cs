using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;

public class MealItemRepository : Interfaces.IMealItemRepository
{
    private readonly MealRecipeDbContext _context;

    public MealItemRepository(MealRecipeDbContext context)
    {
        _context = context;
    }

    private IQueryable<MealItem> WithLookups() =>
        _context.MealItems.Include(mi => mi.ItemType);

    public async Task<MealItem?> GetByIdAsync(Guid id)
    {
        return await WithLookups().FirstOrDefaultAsync(mi => mi.Id == id);
    }

    public async Task<IEnumerable<MealItem>> GetByMealIdAsync(Guid mealId)
    {
        return await WithLookups().Where(mi => mi.MealId == mealId).ToListAsync();
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

    public async Task<bool> DeleteAsync(Guid id)
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