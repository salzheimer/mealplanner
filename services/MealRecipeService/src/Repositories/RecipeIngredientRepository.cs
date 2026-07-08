using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;

public class RecipeIngredientRepository : Interfaces.IRecipeIngredientRepository
{
    private readonly MealRecipeDbContext _context;

    public RecipeIngredientRepository(MealRecipeDbContext context)
    {
        _context = context;
    }
    public async Task<RecipeIngredient?> GetByIdAsync(Guid id)
    {
        return await _context.RecipeIngredients.FindAsync(id);
    }

    public async Task<IEnumerable<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId)
    {
        return await _context.RecipeIngredients.Where(i => i.RecipeId == recipeId).ToListAsync();
    }
    public async Task<RecipeIngredient?> CreateAsync(RecipeIngredient ingredient)
    {
        _context.RecipeIngredients.Add(ingredient);
        var result = await _context.SaveChangesAsync();
        if (result <= 0) return null!;
        return ingredient;
    }
    public async Task<bool> UpdateAsync(RecipeIngredient ingredient)
    {
        _context.Entry(ingredient).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var ingredient = await _context.RecipeIngredients.FindAsync(id);
        if (ingredient == null)
        {
            throw new InvalidOperationException("Ingredient not found");
        }

        _context.RecipeIngredients.Remove(ingredient);

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}