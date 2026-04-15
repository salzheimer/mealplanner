namespace Shared.Models;

public record RecipeDto(
    int Id,
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    int? OwnerUserId,
    Visibility? Visibility,
    IEnumerable<RecipeIngredientDto>? Ingredients,
    IEnumerable<RecipeInstructionDto>? Instructions
);

public record RecipeIngredientDto(
    int Id,
    string? Name,
    decimal? Amount,
    string? MeasurementType
);

public record RecipeInstructionDto(
    int Id,
    int? StepNumber,
    string? Description,
    string? Note
);

public record RecipeCreateDto(
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    Visibility? Visibility
);

public record RecipeUpdateDto(
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    Visibility? Visibility
);
