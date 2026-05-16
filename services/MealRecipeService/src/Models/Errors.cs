using Shared.Models;

public static class RecipeErrors
{
    public static readonly Error NotFound = new("Recipe.NotFound", "Recipe not found.", ErrorType.NotFound);
    public static readonly Error UnableToUpdate = new("Recipe.UnableToUpdate", "Failed to update recipe.", ErrorType.Failure);
    public static readonly Error UnableToCreate = new("Recipe.UnableToCreate", "Failed to create recipe.", ErrorType.Failure);
    public static readonly Error UnableToDelete = new("Recipe.UnableToDelete", "Failed to delete recipe.", ErrorType.Failure);
    public static readonly Error Unauthorized = new("Recipe.Unauthorized", "You do not have permission to access this recipe.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new("Recipe.InvalidInput", "Invalid input data for recipe.", ErrorType.InvalidInput);
    public static readonly Error ShareNotFound = new("RecipeShare.NotFound", "Recipe share not found.", ErrorType.NotFound);
    public static readonly Error ShareUnauthorized = new("RecipeShare.Unauthorized", "You do not have permission to access this recipe share.", ErrorType.Unauthorized);
    public static readonly Error ShareInvalidInput = new("RecipeShare.InvalidInput", "Invalid input data for recipe share.", ErrorType.InvalidInput);
}
public static class RecipeIngredientErrors
{
    public static readonly Error NotFound = new("RecipeIngredient.NotFound", "Recipe ingredient not found.", ErrorType.NotFound);
    public static readonly Error UnableToUpdate = new("RecipeIngredient.UnableToUpdate", "Failed to update recipe ingredient.", ErrorType.Failure);
    public static readonly Error UnableToCreate = new("RecipeIngredient.UnableToCreate", "Failed to create recipe ingredient.", ErrorType.Failure);
    public static readonly Error UnableToDelete = new("RecipeIngredient.UnableToDelete", "Failed to delete recipe ingredient.", ErrorType.Failure);
    public static readonly Error Unauthorized = new("RecipeIngredient.Unauthorized", "You do not have permission to access this recipe ingredient.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new("RecipeIngredient.InvalidInput", "Invalid input data for recipe ingredient.", ErrorType.InvalidInput);
}
public static class RecipeInstructionErrors
{
    public static readonly Error NotFound = new("RecipeInstruction.NotFound", "Recipe instruction not found.", ErrorType.NotFound);
    public static readonly Error UnableToUpdate = new("RecipeInstruction.UnableToUpdate", "Failed to update recipe instruction.", ErrorType.Failure);
    public static readonly Error UnableToDelete = new("RecipeInstruction.UnableToDelete", "Failed to delete recipe instruction.", ErrorType.Failure);
    public static readonly Error UnableToCreate = new("RecipeInstruction.UnableToCreate", "Failed to create recipe instruction.", ErrorType.Failure);
    public static readonly Error Unauthorized = new("RecipeInstruction.Unauthorized", "You do not have permission to access this recipe instruction.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new("RecipeInstruction.InvalidInput", "Invalid input data for recipe instruction.", ErrorType.InvalidInput);
}
public static class RecipeShareErrors
{
    public static readonly Error NotFound = new("RecipeShare.NotFound", "Recipe share not found.", ErrorType.NotFound);
    public static readonly Error UnableToUpdate = new("RecipeShare.UnableToUpdate", "Failed to update recipe share.", ErrorType.BadRequest);
    public static readonly Error UnableToCreate = new("RecipeShare.UnableToCreate", "Failed to create recipe share.", ErrorType.BadRequest);
    public static readonly Error UnableToDelete = new("RecipeShare.UnableToDelete", "Failed to delete recipe share.", ErrorType.BadRequest);
    public static readonly Error Unauthorized = new("RecipeShare.Unauthorized", "You do not have permission to access this recipe share.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new("RecipeShare.InvalidInput", "Invalid input data for recipe share.", ErrorType.InvalidInput);
}
public static class MealErrors
{
    public static readonly Error NotFound = new("Meal.NotFound", "Meal not found.", ErrorType.NotFound);
    public static readonly Error UnableToUpdate = new("Meal.UnableToUpdate", "Failed to update meal.", ErrorType.BadRequest);
    public static readonly Error UnableToCreate = new("Meal.UnableToCreate", "Failed to create meal.", ErrorType.BadRequest);
    public static readonly Error UnableToDelete = new("Meal.UnableToDelete", "Failed to delete meal.", ErrorType.BadRequest);
    public static readonly Error Unauthorized = new("Meal.Unauthorized", "You do not have permission to access this meal.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new("Meal.InvalidInput", "Invalid input data for meal.", ErrorType.InvalidInput);
}



public static class MealItemErrors
{
    public static readonly Error NotFound = new("MealItem.NotFound", "Meal item not found.", ErrorType.NotFound);
    public static readonly Error UnableToUpdate = new("MealItem.UnableToUpdate", "Failed to update meal item.", ErrorType.BadRequest);
    public static readonly Error UnableToCreate = new("MealItem.UnableToCreate", "Failed to create meal item.", ErrorType.BadRequest);
    public static readonly Error UnableToDelete = new("MealItem.UnableToDelete", "Failed to delete meal item.", ErrorType.BadRequest);
    public static readonly Error NotFoundMeal = new("MealItem.MealNotFound", "Associated meal not found.", ErrorType.NotFound);
    public static readonly Error NotFoundRecipe = new("MealItem.RecipeNotFound", "Associated recipe not found.", ErrorType.NotFound);
    public static readonly Error Unauthorized = new("MealItem.Unauthorized", "You do not have permission to access this meal item.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new("MealItem.InvalidInput", "Invalid input data for meal item.", ErrorType.InvalidInput);
}

public static class MealShareErrors
{
    public static readonly Error NotFound = new("MealShare.NotFound", "Meal share not found.", ErrorType.NotFound);
    public static readonly Error UnableToUpdate = new("MealShare.UnableToUpdate", "Failed to update meal share.", ErrorType.BadRequest);
    public static readonly Error UnableToCreate = new("MealShare.UnableToCreate", "Failed to create meal share.", ErrorType.BadRequest);
    public static readonly Error UnableToDelete = new("MealShare.UnableToDelete", "Failed to delete meal share.", ErrorType.BadRequest);
    public static readonly Error NotFoundMeal = new("MealShare.MealNotFound", "Associated meal not found.", ErrorType.NotFound);
    public static readonly Error NotFoundPlan = new("MealShare.PlanNotFound", "Associated plan not found.", ErrorType.NotFound);
    public static readonly Error Unauthorized = new("MealShare.Unauthorized", "You do not have permission to access this meal share.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new("MealShare.InvalidInput", "Invalid input data for meal share.", ErrorType.InvalidInput);
}
