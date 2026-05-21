using MealRecipeService.Models;
using Shared.Models;

namespace MealRecipeService.Interfaces;

public interface IRecipeService
{
    //Recipe operations
    Task<Result<RecipeSummaryDto>> GetRecipeByIdAsync(int userId, int id);
    Task<Result<IEnumerable<RecipeSummaryDto>>> GetAllRecipesAsync(int userId);
    Task<Result<IEnumerable<RecipeSummaryDto>>> GetRecipesSharedWithMeAsync(int userId);
    
    Task<Result<IEnumerable<RecipeSummaryDto>>> GetRecipesByOwnerIdAsync(int userId);
    Task<Result<RecipeDetailDto>> GetRecipeDetailAsync(int userId, int recipeId);
    Task<Result<RecipeSummaryDto>> CreateRecipeAsync(int userId, RecipeCreateDto recipe);
    Task<Result<RecipeSummaryDto>> UpdateRecipeAsync(int userId, int recipeId, RecipeUpdateDto recipe);
    Task<Result<bool>> DeleteRecipeAsync(int userId, int id);
    Task<Result<RecipeSummaryDto>> CloneRecipeAsync(int userId, int recipeId);
    //Recipe ingredient operations
    Task<Result<IEnumerable<RecipeIngredientSummaryDto>>> GetIngredientsByRecipeIdAsync(int userId, int recipeId);
    Task<Result<RecipeIngredientSummaryDto>> AddIngredientToRecipeAsync(int userId, int recipeId, RecipeIngredientCreateDto ingredient);
    Task<Result<RecipeIngredientSummaryDto>> UpdateRecipeIngredientAsync(int userId, int ingredientId, int recipeId, RecipeIngredientUpdateDto ingredient);
    Task<Result<bool>> DeleteRecipeIngredientAsync(int userId, int recipeId, int ingredientId);
    //Recipe instruction operations
    Task<Result<IEnumerable<RecipeInstructionDto>>> GetInstructionsByRecipeIdAsync(int userId, int recipeId);
    Task<Result<RecipeInstructionDto>> AddInstructionToRecipeAsync(int userId, int recipeId, RecipeInstructionCreateDto instruction);
    Task<Result<RecipeInstructionDto>> UpdateRecipeInstructionAsync(int userId, int recipeId, int instructionId, RecipeInstructionUpdateDto instruction);
    Task<Result<bool>> DeleteRecipeInstructionAsync(int userId, int recipeId, int instructionId);
    //Recipe share operations
    Task<Result<ResourcePermissionDto>> ShareRecipeAsync(int userId, int recipeId, ShareRequestDto request);
}