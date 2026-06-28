using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;
namespace MealRecipeService.Repositories;

public class MealTypeRepository : Interfaces.IMealTypeRepository
{
    private readonly MealRecipeDbContext _context;

    public MealTypeRepository(MealRecipeDbContext context)
    {
        _context = context;
    }

    public async Task<MealType?> GetByIdAsync(int id)
    {
        return await _context.MealTypes.FindAsync(id);
    }

    public async Task<MealType?> GetByNameAsync(string name)
    {
        return await _context.MealTypes.FirstOrDefaultAsync(mt => mt.Name == name);
    }

    public async Task<IEnumerable<MealType>> GetAllAsync()
    {
        return await _context.MealTypes.ToListAsync();
    }

    public async Task<MealType?> CreateAsync(MealType mealType)
    {
        var entry = await _context.MealTypes.AddAsync(mealType);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> UpdateAsync(MealType mealType)
    {
        var existing = await _context.MealTypes.FindAsync(mealType.Id);
        if (existing == null) return false;

        existing.DisplayName = mealType.DisplayName;
        existing.SortOrder = mealType.SortOrder;
        

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var mealType = await _context.MealTypes.FindAsync(id);
        if (mealType == null) return false;

        _context.MealTypes.Remove(mealType);
        var result = await _context.SaveChangesAsync();
        return result > 0;  
        }

}