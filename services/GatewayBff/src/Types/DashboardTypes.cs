namespace GatewayBff.Types;

// GraphQL-facing shape — a meal plan entry with its meal composed in,
// so the client gets in one round trip what DashboardPage.tsx currently
// joins itself from two separate REST calls.
public record DashboardMealPlan(
    Guid Id,
    Guid MealId,
    Guid PlanId,
    DateTime? ServeDate,
    DateTime? EndDate,
    MealSummary? Meal
);

public record MealSummary(
    Guid Id,
    string Name,
    string? Description,
    int MealTypeId,
    bool IsMultiDayMeal,
    Guid OwnerUserId
);

// Wire shapes deserialized from PlanService / MealRecipeService responses.
// Kept local (not project-referenced) so GatewayBff stays a thin HTTP client
// of each service rather than compile-time coupled to their internals.
internal record PlanMealPlanDto(
    Guid Id,
    Guid MealId,
    Guid PlanId,
    DateTime? ServeDate,
    DateTime? EndDate,
    Guid AddedByUserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

internal record MealSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    string? Notes,
    int MealTypeId,
    bool IsMultiDayMeal,
    Guid OwnerUserId
);
