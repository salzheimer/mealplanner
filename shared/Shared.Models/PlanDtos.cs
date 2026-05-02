namespace Shared.Models;

public record PlanDto(
    int Id,
    string? Name,
    int OwnerUserId,
    DateTime StartDate,
    DateTime? EndDate

);

public record PlanCreateDto(
    string? Name,
    DateTime StartDate,
    DateTime? EndDate,
    int OwnerUserId
);

public record PlanUpdateDto(
    int Id,
    string? Name,
    DateTime StartDate,
    DateTime? EndDate

);
// PlanShare DTOs
public record PlanShareDto(
    int Id,
    int PlanId,
    int? SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);
public record PlanShareCreateDto(
    int Id,
    int PlanId,
    int? SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime? ExpiresAt
);
public record PlanShareUpdateDto(
    int Id,
    int PlanId,
    int? SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime? ExpiresAt
);
public record MealPlanDto(
    int Id,
    int MealId,
    int PlanId,
    DateTime? ServeDate,
    DateTime? EndDate,
    int AddedByUserId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record MealPlanCreateDto(
    int MealId,
    int PlanId,
    DateTime? ServeDate,
    DateTime? EndDate,
    int AddedByUserId
);
public record MealPlanUpdateDto(
    int Id,
    int MealId,
    int PlanId,
    DateTime? ServeDate,
    DateTime? EndDate,
    int AddedByUserId
);
public record MealItemPlanDto(
    int Id,
    int MealPlanId,
    int? MealItemId,
    string? AssignedToGuestName,
    int? AssignedToUser,
    ItemStatus? Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record MealItemPlanCreateDto(

    int MealPlanId,
    int MealItemId,
    string? AssignedToGuestName,
    int? AssignedToUser,
    ItemStatus? Status,
    string? Notes
);
public record MealItemPlanUpdateDto(
    int Id,
    int MealPlanId,
    int? MealItemId,
    string? AssignedToGuestName,
    int? AssignedToUser,
    ItemStatus? Status,
    string? Notes

);
