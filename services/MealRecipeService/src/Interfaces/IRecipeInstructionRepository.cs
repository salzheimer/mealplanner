using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeInstructionRepository
{
    Task<RecipeInstruction?> GetByIdAsync(int id);
    Task<IEnumerable<RecipeInstruction>> GetByRecipeIdAsync(int recipeId);
    Task<RecipeInstruction?> CreateAsync(RecipeInstruction instruction);
    Task<bool> UpdateAsync(RecipeInstruction instruction);
    Task<bool> DeleteAsync(int id);
}