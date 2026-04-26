using MealRecipeService.Interfaces;
using MealRecipeService.Models;
namespace MealRecipeService.Repositories;
public class MealShareRepository : IMealShareRepository
{
    private readonly MealDbContext _context;  

    public MealShareRepository(MealDbContext context)
    {
        _context = context;
    }

    public async Task<MealShare?> GetByIdAsync(int id)
    {
        return await _context.MealShares.FindAsync(id);
    }

    public async Task<MealShare?> CreateAsync(MealShare mealShare)
    {
        _context.MealShares.Add(mealShare);
        await _context.SaveChangesAsync();
        return mealShare;
    }

    public async Task<bool> UpdateAsync(MealShare mealShare)
    {
        var existing = await _context.MealShares.FindAsync(mealShare.Id);
        if (existing == null) return false;

        existing.MealId = mealShare.MealId;
        existing.SharedWithUserId = mealShare.SharedWithUserId;
        existing.SharedByUserId = mealShare.SharedByUserId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.MealShares.FindAsync(id);
        if (existing == null) return false;

        _context.MealShares.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<MealShare>> GetByMealIdAsync(int mealId)
    {
        return await Task.FromResult(_context.MealShares.Where(ms => ms.MealId == mealId).ToList());
    }

    public async Task<IEnumerable<MealShare>> GetBySharedWithUserIdAsync(int userId)
    {
        return await Task.FromResult(_context.MealShares.Where(ms => ms.SharedWithUserId == userId).ToList());
    }

    public async Task<IEnumerable<MealShare>> GetBySharedByUserIdAsync(int userId)
    {
        return await Task.FromResult(_context.MealShares.Where(ms => ms.SharedByUserId == userId).ToList());
    }

    public async Task<IEnumerable<MealShare>> GetBySharedWithGroupIdAsync(int groupId)
    {
        return await Task.FromResult(_context.MealShares.Where(ms => ms.SharedWithGroupId == groupId).ToList());    
    }
}    