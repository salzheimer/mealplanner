namespace Shared.Models;

public record PlanDto(
    int Id,
    string? Name,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int? GroupId
);

public record PlanCreateDto(
    string? Name,
    DateOnly? StartDate,
    DateOnly? EndDate,
    int? GroupId
);

public record MealDto(
    int Id,
    MealType? MealType,
    bool? IsMultiDayMeal,
    DateOnly? Date,
    DateOnly? EndDate,
    int PlanId
);

public record MealCreateDto(
    MealType? MealType,
    bool? IsMultiDayMeal,
    DateOnly? Date,
    DateOnly? EndDate,
    int PlanId
);

public record MealItemDto(
    int Id,
    string? Name,
    int MealId,
    int? RecipeId,
    ItemType? ItemType,
    string? AssignedToGuestName,
    int? AssignedToUser,
    ItemStatus? Status,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public record MealItemCreateDto(
    string? Name,
    int MealId,
    int? RecipeId,
    ItemType? ItemType,
    string? AssignedToGuestName,
    int? AssignedToUser,
    ItemStatus? Status,
    string? Notes
);
