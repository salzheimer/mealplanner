namespace MealRecipeService.Contracts;

//Recipe ingredient DTOs
public record RecipeIngredientDetailResponse(
    Guid Id,
    Guid RecipeId,
    string? Name,
    decimal? Amount,
    string? MeasurementType,
    string? Note,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int CreatedBy,
    int UpdatedBy
);
public record RecipeIngredientSummaryResponse(
    Guid Id,
    Guid RecipeId,
    string? Name,
    decimal? Amount,
    string? MeasurementType,
    string? Note
);
public record CreateRecipeIngredientRequest(
    string? Name,
    decimal? Amount,
    string? MeasurementType,
    string? Note
);
public record UpdateRecipeIngredientRequest(
    Guid Id,
    string? Name,
    decimal? Amount,
    string? MeasurementType,
    string? Note
);
