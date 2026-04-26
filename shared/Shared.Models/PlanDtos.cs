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

public record MealPlanDto(
    int Id,
    int MealId,
    int PlanId,
    DateOnly? SearveDate,
    DateOnly? EndDate,
    int AddedByUserId,
    DateTimeOffset CreatedAt
);

public record MealPlanCreateDto(
    int MealId,
    int PlanId,
    DateOnly? SearveDate,
    DateOnly? EndDate,
    int AddedByUserId
);

public record MealItemPlanDto(
    int Id,

    int MealPlanId,
    int? MealItemId,
    ItemType? ItemType,
    string? AssignedToGuestName,
    int? AssignedToUser,
    ItemStatus? Status,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);

public record MealItemPlanCreateDto(

    int MealPlanId,
    int? MealItemId,
    string? AssignedToGuestName,
    int? AssignedToUser,
    ItemStatus? Status,
    string? Notes
);
