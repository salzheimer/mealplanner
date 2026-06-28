using MealRecipeService.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeInstructionRepository
{
    Task<RecipeInstruction?> GetByIdAsync(Guid id);
    Task<IEnumerable<RecipeInstruction>> GetByRecipeIdAsync(Guid recipeId);
    Task<RecipeInstruction?> CreateAsync(RecipeInstruction instruction);
    Task<bool> UpdateAsync(RecipeInstruction instruction);
    Task<bool> DeleteAsync(Guid id);
}