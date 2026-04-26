using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;



public class RecipeRepository : Interfaces.IRecipeRepository
{
    private readonly MealDbContext _context;

    public RecipeRepository(MealDbContext context)
    {
        _context = context;
    }
    public async Task<Recipe?> GetByIdAsync(int id)
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
    public async Task<bool> DeleteAsync(int id)
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

    public async Task<IEnumerable<Recipe>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Recipes.Where(r => r.OwnerUserId == ownerId).ToListAsync();
    }
}