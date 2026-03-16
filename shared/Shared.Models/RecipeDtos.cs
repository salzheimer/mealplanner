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
    int? Servings
);

public record RecipeCreateDto(
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings
);

public record RecipeUpdateDto(
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings
);
