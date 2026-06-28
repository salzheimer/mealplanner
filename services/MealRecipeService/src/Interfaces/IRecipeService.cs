using MealRecipeService.Contracts;
using MealRecipeService.Models;
using Shared.Models;

namespace MealRecipeService.Interfaces;

public interface IRecipeService
{
    //Recipe operations
    Task<Result<RecipeSummaryResponse>> GetRecipeByIdAsync(Guid userId, Guid recipeId);
    Task<Result<IEnumerable<RecipeSummaryResponse>>> GetAllRecipesAsync(Guid userId);
    Task<Result<IEnumerable<RecipeSummaryResponse>>> GetRecipesSharedWithMeAsync(Guid userId);
    
    Task<Result<IEnumerable<RecipeSummaryResponse>>> GetRecipesByOwnerIdAsync(Guid userId);
    Task<Result<RecipeDetailResponse>> GetRecipeDetailAsync(Guid userId, Guid recipeId);
    Task<Result<RecipeSummaryResponse>> CreateRecipeAsync(Guid userId, CreateRecipeRequest recipe);
    Task<Result<RecipeSummaryResponse>> UpdateRecipeAsync(Guid userId, Guid recipeId, UpdateRecipeRequest recipe);
    Task<Result<bool>> DeleteRecipeAsync(Guid userId, Guid recipeId);
    Task<Result<RecipeSummaryResponse>> CloneRecipeAsync(Guid userId, Guid recipeId);
    //Recipe ingredient operations
    Task<Result<IEnumerable<RecipeIngredientSummaryResponse>>> GetIngredientsByRecipeIdAsync(Guid userId, Guid recipeId);
    Task<Result<RecipeIngredientSummaryResponse>> AddIngredientToRecipeAsync(Guid userId, Guid recipeId, CreateRecipeIngredientRequest ingredient);
    Task<Result<RecipeIngredientSummaryResponse>> UpdateRecipeIngredientAsync(Guid userId, Guid ingredientId, Guid recipeId, UpdateRecipeIngredientRequest ingredient);
    Task<Result<bool>> DeleteRecipeIngredientAsync(Guid userId, Guid recipeId, Guid ingredientId);
    //Recipe instruction operations
    Task<Result<IEnumerable<RecipeInstructionResponse>>> GetInstructionsByRecipeIdAsync(Guid userId, Guid recipeId);
    Task<Result<RecipeInstructionResponse>> AddInstructionToRecipeAsync(Guid userId, Guid recipeId, CreateRecipeInstructionRequest instruction);
    Task<Result<RecipeInstructionResponse>> UpdateRecipeInstructionAsync(Guid userId, Guid recipeId, Guid instructionId, UpdateRecipeInstructionRequest instruction);
    Task<Result<bool>> DeleteRecipeInstructionAsync(Guid userId, Guid recipeId, Guid instructionId);
    //Recipe share operations
    Task<Result<ShareRecipeResponse>> ShareRecipeAsync(ShareRecipeRequest shareRecipeRequest);
}