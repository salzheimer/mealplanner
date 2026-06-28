namespace MealRecipeService.Contracts;

public record RecipeDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    Guid? OwnerUserId,
    IEnumerable<RecipeIngredientSummaryResponse>? Ingredients,
    IEnumerable<RecipeInstructionResponse>? Instructions
);
public record RecipeSummaryResponse(
    Guid Id,
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    Guid? OwnerUserId
);




public record CreateRecipeRequest(
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    Guid? OwnerUserId
);

public record UpdateRecipeRequest(
    Guid Id,
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    Guid? OwnerUserId
);




public record ShareRecipeRequest(
    Guid RecipeId,
    string SubjectTypeName,
    Guid SubjectId,
    string PermissionTypeName,
    Guid GrantedBy,
    DateTimeOffset? ExpiresAt
);
public record ShareRecipeResponse(
    Guid RecipeId,
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
