using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;

public class RecipeIngredientRepository : Interfaces.IRecipeIngredientRepository
{
    private readonly MealDbContext _context;

    public RecipeIngredientRepository(MealDbContext context)
    {
        _context = context;
    }
    public async Task<RecipeIngredient?> GetByIdAsync(int id)
    {
        return await _context.RecipeIngredients.FindAsync(id);
    }

    public async Task<IEnumerable<RecipeIngredient>> GetByRecipeIdAsync(int recipeId)
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
    public async Task<bool> DeleteAsync(int id)
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