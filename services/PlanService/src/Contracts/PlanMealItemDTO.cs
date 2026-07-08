public record PlanMealItemResponse(
    Guid Id,
    Guid MealPlanId,
    Guid? MealItemId,
    string? AssignedToGuestName,
    Guid? AssignedToUser,
    int? StatusTypeId,
    string StatusTypeName,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreatePlanMealItemRequest(

    Guid MealPlanId,
    Guid MealItemId,
    string? AssignedToGuestName,
    Guid? AssignedToUser,
    int? StatusTypeId,
    string StatusTypeName,
    string? Notes
);
public record UpdatePlanMealItemRequest(
    Guid Id,
    Guid MealPlanId,
    Guid? MealItemId,
    string? AssignedToGuestName,
    Guid? AssignedToUser,
    int? StatusTypeId,
    string StatusTypeName,
    string? Notes

);