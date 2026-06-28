using MealRecipeService.Models;
using Microsoft.EntityFrameworkCore;

namespace MealRecipeService.Repositories;

public class RecipeInstructionRepository : Interfaces.IRecipeInstructionRepository
{
    private readonly MealRecipeDbContext _context;

    public RecipeInstructionRepository(MealRecipeDbContext context)
    {
        _context = context;
    }
    public async Task<RecipeInstruction?> GetByIdAsync(Guid id)
    {
        return await _context.RecipeInstructions.FindAsync(id);
    }

    public async Task<IEnumerable<RecipeInstruction>> GetByRecipeIdAsync(Guid recipeId)
    {
        return await _context.RecipeInstructions.Where(i => i.RecipeId == recipeId).ToListAsync();
    }
    public async Task<RecipeInstruction?> CreateAsync(RecipeInstruction instruction)
    {
        _context.RecipeInstructions.Add(instruction);
       var result = await _context.SaveChangesAsync();
       if (result <= 0) return null!;
        return instruction;
    }
    public async Task<bool> UpdateAsync(RecipeInstruction instruction)
    {
        _context.Entry(instruction).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var instruction = await _context.RecipeInstructions.FindAsync(id);
        if (instruction == null)
        {
            return false;
        }
        _context.RecipeInstructions.Remove(instruction);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}