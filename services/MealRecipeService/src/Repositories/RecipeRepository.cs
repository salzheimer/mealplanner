using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;



public class RecipeRepository : Interfaces.IRecipeRepository
{
    private readonly MealRecipeDbContext _context;

    public RecipeRepository(MealRecipeDbContext context)
    {
        _context = context;
    }
    public async Task<Recipe?> GetByIdAsync(Guid id)
    {
        return await _context.Recipes.FindAsync(id);
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync()
    {
        return await _context.Recipes.ToListAsync();
    }
    public async Task<Recipe?> CreateAsync(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
       var result = await _context.SaveChangesAsync();
        if (result <= 0) return null!;
        return recipe;
    }
    public async Task<bool> UpdateAsync(Recipe recipe)
    {
        _context.Entry(recipe).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
        {
            return false;
        }
        _context.Recipes.Remove(recipe);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<Recipe>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Recipes.Where(r => r.OwnerUserId == ownerId).ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetByIdsAsync(HashSet<Guid> recipeIds)
    {
       return await _context.Recipes.Where(r=> recipeIds.Contains(r.Id)).ToListAsync();
    }
}