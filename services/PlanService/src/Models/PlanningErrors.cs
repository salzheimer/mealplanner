using Shared.Models;

namespace PlanService.Models;
public static class PlanningErrors
{
    public static readonly Error PlanNotFound = new ("Plan.NotFound", "The specified plan was not found.");
    public static readonly Error PlanShareNotFound = new ("PlanShare.NotFound", "The specified plan share was not found.");
    public static readonly Error Unauthorized = new ("Plan.Unauthorized", "You do not have permission to perform this action.");
    public static readonly Error InvalidInput = new ("Plan.InvalidInput", "The provided input is invalid.");
    public static readonly Error UnableToCreate = new ("Plan.UnableToCreate", "Failed to create the plan.");
    public static readonly Error UnableToUpdate = new ("Plan.UnableToUpdate", "Failed to update the plan.");
    public static readonly Error UnableToDelete = new ("Plan.UnableToDelete", "Failed to delete the plan.");
    public static readonly Error UnableToShare = new ("Plan.UnableToShare", "Failed to share the plan.");
}