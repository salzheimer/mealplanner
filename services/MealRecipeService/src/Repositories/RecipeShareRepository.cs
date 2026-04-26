using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;


namespace MealRecipeService.Repositories;

public class RecipeShareRepository : Interfaces.IRecipeShareRepository
{
    private readonly MealDbContext _context;

    public RecipeShareRepository(MealDbContext context)
    {
        _context = context;
    }

    public async Task<RecipeShare?> GetByIdAsync(int id)
    {
        return await _context.RecipeShares.FindAsync(id);
    }

    public async Task<IEnumerable<RecipeShare>> GetByRecipeIdAsync(int recipeId)
    {
        return await _context.RecipeShares.Where(rs => rs.RecipeId == recipeId).ToListAsync();
    }

    public async Task<RecipeShare?> CreateAsync(RecipeShare share)
    {
        _context.RecipeShares.Add(share);
      var result = await _context.SaveChangesAsync();
        if (result <= 0) return null!;
        return share;
    }

    public async Task<bool> UpdateAsync(RecipeShare share)
    {
        _context.Entry(share).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var share = await _context.RecipeShares.FindAsync(id);
        if (share == null)
        {
            return false;
        }
        _context.RecipeShares.Remove(share);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<RecipeShare>> GetBySharedWithUserIdAsync(int userId)
    {
        return await _context.RecipeShares.Where(rs => rs.SharedWithUserId == userId).ToListAsync();

    }

    public async Task<IEnumerable<RecipeShare>> GetBySharedWithGroupIdAsync(int groupId)
    {
        return await _context.RecipeShares.Where(rs => rs.SharedWithGroupId == groupId).ToListAsync();
    }
}