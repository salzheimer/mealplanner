using Shared.Models;

namespace PlanService.Models;

public static class MealItemPlanErrors
{
    public static readonly Error MealItemPlanNotFound = new ("MealItemPlan.NotFound", "The specified meal item plan was not found.", ErrorType.NotFound);
    public static readonly Error Unauthorized = new ("MealItemPlan.Unauthorized", "You do not have permission to perform this action.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new ("MealItemPlan.InvalidInput", "The provided input is invalid.", ErrorType.InvalidInput);
    public static readonly Error UnableToCreate = new ("MealItemPlan.UnableToCreate", "Failed to create the meal item plan.", ErrorType.BadRequest);
    public static readonly Error UnableToUpdate = new ("MealItemPlan.UnableToUpdate", "Failed to update the meal item plan.", ErrorType.BadRequest);
    public static readonly Error UnableToDelete = new ("MealItemPlan.UnableToDelete", "Failed to delete the meal item plan.", ErrorType.BadRequest);
}
public static class MealItemStatusErrors
{
    public static readonly Error MealItemStatusNotFound = new ("MealItemStatus.NotFound", "The specified meal item status was not found.", ErrorType.NotFound);
    public static readonly Error Unauthorized = new ("MealItemStatus.Unauthorized", "You do not have permission to perform this action.", ErrorType.Unauthorized);
    public static readonly Error InvalidInput = new ("MealItemStatus.InvalidInput", "The provided input is invalid.", ErrorType.InvalidInput);
    public static readonly Error UnableToCreate = new ("MealItemStatus.UnableToCreate", "Failed to create the meal item status.", ErrorType.BadRequest);
    public static readonly Error UnableToUpdate = new ("MealItemStatus.UnableToUpdate", "Failed to update the meal item status.", ErrorType.BadRequest);
    public static readonly Error UnableToDelete = new ("MealItemStatus.UnableToDelete", "Failed to delete the meal item status.", ErrorType.BadRequest);
}