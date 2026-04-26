namespace Shared.Models;
//Recipe  DTOs
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
public record RecipeSummaryDto(
    int Id,
    string Name,
    string? Description,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    int? OwnerUserId,
    Visibility? Visibility
);

public record RecipeIngredientDto(
    int Id,
    int RecipeId,
    string? Name,
    decimal? Amount,
    string? MeasurementType
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
    int? OwnerUserId,
    Visibility? Visibility
);

public record RecipeUpdateDto(
    int Id,
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
//Recipe ingredient DTOs
public record RecipeInstructionDto(
    int Id,
    int RecipeId,
    int? StepNumber,
    string? Description,
    string? Note
);
//Recipe share DTOs
public record RecipeShareDto(
    int Id,
    int RecipeId,
    int SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime SharedAt
);
public record RecipeShareCreateDto(
    int RecipeId,
    int? SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime? ExpiresAt
);
public record RecipeShareUpdateDto(
    int Id,
    int RecipeId,
    int? SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime? ExpiresAt
);

//Meal DTOs
public record MealDto(
    int Id,
    string ? Name,
    string? Description,
    string? Notes,
    MealType MealType,
    bool IsMultiDayMeal,
    Visibility? Visibility,
    DateTime? CreateAt,
    DateTime? UpdatedAt
    
);

public record MealCreateDto(
    MealType MealType,
    string  Name,
    string? Description,
    string? Notes,
    bool? IsMultiDayMeal,
    Visibility? Visibility
    
     
);
public record MealUpdateDto(
    int Id,
    MealType MealType,
    string  Name,
    string? Description,
    string? Notes,
    bool? IsMultiDayMeal,
    Visibility? Visibility,    
    DateTime? UpdatedAt
     
);
public record MealItemDto(
    int Id,
    string Name,
    int MealId,
    int? RecipeId,
    ItemType? ItemType
    
);

public record MealItemCreateDto(
    string Name,
    int MealId,
    int? RecipeId,
    ItemType? ItemType
   
);
public record MealItemUpdateDto(
    int Id,
    string Name,
    int MealId,
    int? RecipeId,
    ItemType? ItemType
   
);
//Meal share DTOs
public record MealShareDto(
    int Id,
    int MealId,
    int? SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);
public record MealShareCreateDto(
    int MealId,
    int? SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime? ExpiresAt
);
public record MealShareUpdateDto(
    int Id,
    int MealId,
    int? SharedWithUserId,
    int? SharedWithGroupId,
    int SharedByUserId,
    Permission Permission,
    DateTime? ExpiresAt
);