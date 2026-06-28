using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;

public class RecipeComponentRepository : Interfaces.IRecipeComponentRepository
{
    private readonly MealRecipeDbContext _context;

    public RecipeComponentRepository(MealRecipeDbContext context)
    {
        _context = context;
    }

    public async Task<RecipeComponent?> GetByIdAsync(Guid id)
    {
        return await _context.RecipeComponents.FindAsync(id);
    }

    public async Task<IEnumerable<RecipeComponent>> GetChildrenRecipesAsync(Guid parentRecipeId)
    {
        return await _context.RecipeComponents.Where(rc => rc.ParentRecipeId == parentRecipeId).ToListAsync();
    }

    public async Task<RecipeComponent?> CreateAsync(RecipeComponent component)
    {
        var entry = await _context.RecipeComponents.AddAsync(component);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> UpdateAsync(RecipeComponent component)
    {
        var existing = await _context.RecipeComponents.FindAsync(component.Id);
        if (existing == null) return false;

        existing.ChildRecipeId = component.ChildRecipeId;
        existing.SortOrder = component.SortOrder;
        existing.ParentRecipeId = component.ParentRecipeId;
        existing.AssemblyNotes = component.AssemblyNotes;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var component = await _context.RecipeComponents.FindAsync(id);
        if (component == null) return false;

        _context.RecipeComponents.Remove(component);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}