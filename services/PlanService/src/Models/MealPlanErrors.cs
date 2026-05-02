using Shared.Models;

namespace PlanService.Models;

public static class MealPlanErrors
{
    public static readonly Error MealPlanNotFound = new ("MealPlan.NotFound", "The specified meal plan was not found.");
    public static readonly Error Unauthorized = new ("MealPlan.Unauthorized", "You do not have permission to perform this action.");
    public static readonly Error InvalidInput = new ("MealPlan.InvalidInput", "The provided input is invalid.");
    public static readonly Error UnableToCreate = new ("MealPlan.UnableToCreate", "Failed to create the meal plan.");
    public static readonly Error UnableToUpdate = new ("MealPlan.UnableToUpdate", "Failed to update the meal plan.");
    public static readonly Error UnableToDelete = new ("MealPlan.UnableToDelete", "Failed to delete the meal plan.");
}