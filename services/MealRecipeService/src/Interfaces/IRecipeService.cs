using MealRecipeService.Models;
using Shared.Models;

namespace MealRecipeService.Interfaces;

public interface IRecipeService
{
    //Recipe operations
    Task<Result<RecipeDto>> GetRecipeByIdAsync(int userId, int id);
    Task<Result<IEnumerable<RecipeSummaryDto>>> GetAllRecipesAsync(int userId);
    Task<Result<IEnumerable<RecipeSummaryDto>>> GetRecipesByOwnerIdAsync(int userId);
    Task<Result<RecipeSummaryDto>> CreateRecipeAsync(int userId, RecipeCreateDto recipe);
    Task<Result<RecipeSummaryDto>> UpdateRecipeAsync(int userId, RecipeUpdateDto recipe);
    Task<Result<bool>> DeleteRecipeAsync(int userId, int id);
    Task<Result<RecipeSummaryDto>> CloneRecipeAsync(int userId, int recipeId);
    //Recipe ingredient operations
    Task<Result<IEnumerable<RecipeIngredientSummaryDto>>> GetIngredientsByRecipeIdAsync(int userId, int recipeId);
    Task<Result<RecipeIngredientSummaryDto>> AddIngredientToRecipeAsync(int userId, RecipeIngredientCreateDto ingredient);
    Task<Result<RecipeIngredientSummaryDto>> UpdateRecipeIngredientAsync(int userId, RecipeIngredientSummaryDto ingredient);
    Task<Result<bool>> DeleteRecipeIngredientAsync(int userId, int recipeId, int ingredientId);
    //Recipe instruction operations
    Task<Result<IEnumerable<RecipeInstructionDto>>> GetInstructionsByRecipeIdAsync(int userId, int recipeId);
    Task<Result<RecipeInstructionDto>> AddInstructionToRecipeAsync(int userId, RecipeInstructionCreateDto instruction);
    Task<Result<RecipeInstructionDto>> UpdateRecipeInstructionAsync(int userId, RecipeInstructionDto instruction);
    Task<Result<bool>> DeleteRecipeInstructionAsync(int userId, int recipeId, int instructionId);
    //Recipe share operations
    Task<Result<ResourcePermissionDto>> ShareRecipeAsync(int userId, int recipeId, ShareRequestDto request);
}