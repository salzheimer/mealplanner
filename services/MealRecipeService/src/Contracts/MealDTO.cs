namespace MealRecipeService.Contracts;

public record CreateMealRequest(
    string Name,
    string? Description,
    string? Notes,
    int MealTypeId,
    bool IsMultiDayMeal,
    Guid OwnerUserId
);
public record UpdateMealRequest(
    Guid Id,
    string Name,
    string? Description,
    string? Notes,
    int MealTypeId,
    bool IsMultiDayMeal,
    Guid UpdatedBy
);
public record ShareMealRequest(
    Guid MealId,
    string SubjectTypeName,
    Guid SubjectId,
    string PermissionTypeName,
    Guid GrantedBy,
    DateTimeOffset? ExpiresAt
);
public record ShareMealResponse(
    Guid MealId,
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
/// <summary>
/// List response
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="Notes"></param>
/// <param name="MealType"></param>
/// <param name="IsMultiDayMeal"></param>
/// <param name="OwnerUserId"></param>
public record MealSummaryResponse(
    Guid Id,
    string Name,
    string? Description,
    string? Notes,
    int MealTypeId,
    bool IsMultiDayMeal,
    Guid OwnerUserId
     

);
public record MealDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    string? Notes,
    int MealTypeId,
    bool IsMultiDayMeal,
    Guid OwnerUserId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid UpdatedBy

);
public record MealTypeResponse(
    int Id,
    string Name,
    string DisplayName,
    int SortOrder
);
public record CreateMealType(
    string Name,
    string DisplayName,
    int SortOrder
);
