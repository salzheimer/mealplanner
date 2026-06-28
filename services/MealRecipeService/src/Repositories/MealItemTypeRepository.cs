using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;
namespace MealRecipeService.Repositories;

public class MealItemTypeRepository : Interfaces.IMealItemTypeRepository
{
    private readonly MealRecipeDbContext _context;

    public MealItemTypeRepository(MealRecipeDbContext context)
    {
        _context = context;
    }

    public async Task<MealItemType?> GetByIdAsync(int id)
    {
        return await _context.MealItemTypes.FindAsync(id);
    }

    public async Task<MealItemType?> GetByNameAsync(string name)
    {
        return await _context.MealItemTypes.FirstOrDefaultAsync(m => m.Name == name);
    }

    public async Task<IEnumerable<MealItemType>> GetAllAsync()
    {
        return await _context.MealItemTypes.ToListAsync();
    }

    public async Task<MealItemType?> CreateAsync(MealItemType mealItemType)
    {
        _context.MealItemTypes.Add(mealItemType);
        var result = await _context.SaveChangesAsync();
        if (result > 0)
        {
            return mealItemType;
        }
        return null;
    }

    public async Task<bool> UpdateAsync(MealItemType mealItemType)
    {
        var existing = await _context.MealItemTypes.FindAsync(mealItemType.Id);
        if (existing == null)
        {
            return false;
        }
        existing.DisplayName = mealItemType.DisplayName;
        existing.SortOrder = mealItemType.SortOrder;

        var result = await _context.SaveChangesAsync();
        return result > 0;

    }
    public async Task<bool> DeleteAsync(int id)
    {
        var mealItemType = await _context.MealItemTypes.FindAsync(id);
        if (mealItemType == null)
        {
            return false;
        }
        _context.MealItemTypes.Remove(mealItemType);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

}
