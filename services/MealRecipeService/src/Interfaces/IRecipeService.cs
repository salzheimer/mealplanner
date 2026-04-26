using MealRecipeService.Models;
using Shared.Models;

namespace MealRecipeService.Interfaces;
public interface IRecipeService
{
    //Recipe operations
    Task<Result<RecipeDto>> GetRecipeByIdAsync(int id);
    Task<Result<IEnumerable<RecipeSummaryDto>>> GetAllRecipesAsync();
    Task<Result<IEnumerable<RecipeSummaryDto>>> GetRecipesByOwnerIdAsync(int userId);
    Task<Result<RecipeSummaryDto>> CreateRecipeAsync(RecipeCreateDto recipe);
    Task<Result<RecipeSummaryDto>> UpdateRecipeAsync(RecipeUpdateDto recipe);
    Task<Result<bool>> DeleteRecipeAsync(int id);

    //Recipe ingredient operations
    Task<Result<IEnumerable<RecipeIngredientDto>>> GetIngredientsByRecipeIdAsync(int recipeId);
    Task<Result<RecipeIngredientDto>> AddIngredientToRecipeAsync(RecipeIngredientDto ingredient);
    Task<Result<RecipeIngredientDto>> UpdateRecipeIngredientAsync(RecipeIngredientDto ingredient);
    Task<Result<bool>> DeleteRecipeIngredientAsync(int id);
    //Recipe instruction operations
    Task<Result<IEnumerable<RecipeInstructionDto>>> GetInstructionsByRecipeIdAsync(int recipeId);
    Task<Result<RecipeInstructionDto>> AddInstructionToRecipeAsync(RecipeInstructionDto instruction);
    Task<Result<RecipeInstructionDto>> UpdateRecipeInstructionAsync(RecipeInstructionDto instruction);
    Task<Result<bool>> DeleteRecipeInstructionAsync(int id);
    //Recipe share operations   
    Task<Result<RecipeShareDto>> GetShareByIdAsync(int id);
    Task<Result<IEnumerable<RecipeShareDto>>> GetShareByRecipeIdAsync(int recipeId);
    Task<Result<IEnumerable<RecipeShareDto>>> GetSharesBySharedWithUserIdAsync(int userId);
    Task<Result<IEnumerable<RecipeShareDto>>> GetSharesBySharedWithGroupIdAsync(int groupId);
    Task<Result<RecipeShareDto>> CreateShareAsync(RecipeShareCreateDto share);
    Task<Result<RecipeShareDto>> UpdateShareAsync(RecipeShareUpdateDto share);
    Task<Result<bool>> DeleteShareAsync(int id);
}