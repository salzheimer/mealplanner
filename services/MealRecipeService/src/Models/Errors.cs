using Shared.Models;

public static class RecipeErrors
{
    public static readonly Error NotFound = new("Recipe.NotFound", "Recipe not found.");
    public static readonly Error UnableToUpdate = new("Recipe.UnableToUpdate", "Failed to update recipe.");
    public static readonly Error UnableToCreate = new("Recipe.UnableToCreate", "Failed to create recipe.");
    public static readonly Error UnableToDelete = new("Recipe.UnableToDelete", "Failed to delete recipe.");
    public static readonly Error Unauthorized = new("Recipe.Unauthorized", "You do not have permission to access this recipe.");
    public static readonly Error InvalidInput = new("Recipe.InvalidInput", "Invalid input data for recipe.");
    public static readonly Error ShareNotFound = new("RecipeShare.NotFound", "Recipe share not found.");
    public static readonly Error ShareUnauthorized = new("RecipeShare.Unauthorized", "You do not have permission to access this recipe share.");
    public static readonly Error ShareInvalidInput = new("RecipeShare.InvalidInput", "Invalid input data for recipe share.");
}
public static class RecipeIngredientErrors
{
    public static readonly Error NotFound = new("RecipeIngredient.NotFound", "Recipe ingredient not found.");
    public static readonly Error UnableToUpdate = new("RecipeIngredient.UnableToUpdate", "Failed to update recipe ingredient.");
    public static readonly Error UnableToCreate = new("RecipeIngredient.UnableToCreate", "Failed to create recipe ingredient.");
    public static readonly Error UnableToDelete = new("RecipeIngredient.UnableToDelete", "Failed to delete recipe ingredient.");
    public static readonly Error Unauthorized = new("RecipeIngredient.Unauthorized", "You do not have permission to access this recipe ingredient.");
    public static readonly Error InvalidInput = new("RecipeIngredient.InvalidInput", "Invalid input data for recipe ingredient.");
}
public static class RecipeInstructionErrors
{
    public static readonly Error NotFound = new("RecipeInstruction.NotFound", "Recipe instruction not found.");
    public static readonly Error UnableToUpdate = new("RecipeInstruction.UnableToUpdate", "Failed to update recipe instruction.");
    public static readonly Error UnableToDelete = new("RecipeInstruction.UnableToDelete", "Failed to delete recipe instruction.");
    public static readonly Error UnableToCreate = new("RecipeInstruction.UnableToCreate", "Failed to create recipe instruction.");
    public static readonly Error Unauthorized = new("RecipeInstruction.Unauthorized", "You do not have permission to access this recipe instruction.");
    public static readonly Error InvalidInput = new("RecipeInstruction.InvalidInput", "Invalid input data for recipe instruction.");
}
public static class RecipeShareErrors
{
    public static readonly Error NotFound = new("RecipeShare.NotFound", "Recipe share not found.");
    public static readonly Error UnableToUpdate = new("RecipeShare.UnableToUpdate", "Failed to update recipe share.");
    public static readonly Error UnableToCreate = new("RecipeShare.UnableToCreate", "Failed to create recipe share.");
    public static readonly Error UnableToDelete = new("RecipeShare.UnableToDelete", "Failed to delete recipe share.");
    public static readonly Error Unauthorized = new("RecipeShare.Unauthorized", "You do not have permission to access this recipe share.");
    public static readonly Error InvalidInput = new("RecipeShare.InvalidInput", "Invalid input data for recipe share.");
}
public static class MealErrors
{
    public static readonly Error NotFound = new("Meal.NotFound", "Meal not found.");
    public static readonly Error UnableToUpdate = new("Meal.UnableToUpdate", "Failed to update meal.");
    public static readonly Error UnableToCreate = new("Meal.UnableToCreate", "Failed to create meal.");
    public static readonly Error UnableToDelete = new("Meal.UnableToDelete", "Failed to delete meal.");
    public static readonly Error Unauthorized = new("Meal.Unauthorized", "You do not have permission to access this meal.");
    public static readonly Error InvalidInput = new("Meal.InvalidInput", "Invalid input data for meal.");
}



public static class MealItemErrors
{
    public static readonly Error NotFound = new("MealItem.NotFound", "Meal item not found.");
    public static readonly Error UnableToUpdate = new("MealItem.UnableToUpdate", "Failed to update meal item.");
    public static readonly Error UnableToCreate = new("MealItem.UnableToCreate", "Failed to create meal item.");
    public static readonly Error UnableToDelete = new("MealItem.UnableToDelete", "Failed to delete meal item.");
   public static readonly Error NotFoundMeal = new("MealItem.MealNotFound", "Associated meal not found.");
    public static readonly Error NotFoundRecipe = new("MealItem.RecipeNotFound", "Associated recipe not found.");
    public static readonly Error Unauthorized = new("MealItem.Unauthorized", "You do not have permission to access this meal item.");
    public static readonly Error InvalidInput = new("MealItem.InvalidInput", "Invalid input data for meal item.");
}

public static class MealShareErrors
{
    public static readonly Error NotFound = new("MealShare.NotFound", "Meal share not found.");
    public static readonly Error UnableToUpdate = new("MealShare.UnableToUpdate", "Failed to update meal share.");
    public static readonly Error UnableToCreate = new("MealShare.UnableToCreate", "Failed to create meal share.");
    public static readonly Error UnableToDelete = new("MealShare.UnableToDelete", "Failed to delete meal share.");
    public static readonly Error Unauthorized = new("MealShare.Unauthorized", "You do not have permission to access this meal share.");
    public static readonly Error InvalidInput = new("MealShare.InvalidInput", "Invalid input data for meal share.");
}
