public record PlanSummaryResponse(
    Guid Id,
    string? Name,
    Guid OwnerUserId,
    DateTime StartDate,
    DateTime? EndDate

);
public record PlanDetailResponse(
    Guid Id,
    string? Name,
    Guid OwnerUserId,
    DateTime StartDate,
    DateTime? EndDate,
    Guid CreatedBy,
    Guid UpdatedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt

);
public record CreatePlanRequest(
    string? Name,
    DateTime StartDate,
    DateTime? EndDate

);

public record UpdatePlanRequest(
    Guid Id,
    string? Name,
    DateTime StartDate,
    DateTime? EndDate

);

public record SharePlanResponse(
    Guid PlanId,
    string? ResourceTypeName,
    int ResourceTypeId,
    string? SubjectTypeName,
    int SubjectTypeId,
    string? PermissionTypeName,
    int PermissionTypeId,
    Guid SubjectId,
    Guid GrantedBy,
    DateTimeOffset? ExpiresAt
);

public record SharePlanRequest(
    Guid PlanId,
    string SubjectTypeName,
    Guid SubjectId,
    string PermissionTypeName,
    Guid GrantedBy,
    DateTimeOffset? ExpiresAt
);