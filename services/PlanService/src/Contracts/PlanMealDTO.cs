namespace PlanService.Contracts;
public record PlanMealResponse(
    Guid Id,
    Guid MealId,
    Guid PlanId,
    DateTime? ServeDate,
    DateTime? EndDate,
    Guid AddedByUserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreatePlanMealRequest(
    Guid MealId,
    Guid PlanId,
    DateTime? ServeDate,
    DateTime? EndDate,
    Guid AddedByUserId
);
public record UpdatePlanMealRequest(
    Guid Id,
    Guid MealId,
    Guid PlanId,
    DateTime? ServeDate,
    DateTime? EndDate,
    Guid AddedByUserId
);