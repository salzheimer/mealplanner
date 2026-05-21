namespace Shared.Models;
//Recipe  DTOs
public record RecipeDetailDto(
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
    IEnumerable<RecipeIngredientSummaryDto>? Ingredients,
    IEnumerable<RecipeInstructionDto>? Instructions
);
public record RecipeSummaryDto(
    int Id,
    string Name,
    string? Description,
    string? Notes,
    int? Ranking,
    string? OriginalSource,
    TimeSpan? CookTime,
    TimeSpan? PrepTime,
    int? Servings,
    int? OwnerUserId
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
    int? OwnerUserId
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
    int? OwnerUserId
);

//Recipe ingredient DTOs
public record RecipeIngredientDetailDto(
    int Id,
    int RecipeId,
    string? Name,
    decimal? Amount,
    string? MeasurementType,
    string? Note,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int CreatedBy,
    int UpdatedBy
);
public record RecipeIngredientSummaryDto(
    int Id,
    int RecipeId,
    string? Name,
    decimal? Amount,
    string? MeasurementType,
    string? Note
);
public record RecipeIngredientCreateDto(
    string? Name,
    decimal? Amount,
    string? MeasurementType,
    string? Note
);
public record RecipeIngredientUpdateDto(
    int Id,
    string? Name,
    decimal? Amount,
    string? MeasurementType,
    string? Note
);

//Recipe instructions DTOs
public record RecipeInstructionDto(
    int Id,
    int RecipeId,
    int? StepNumber,
    string? Description,
    string? Note
);
public record RecipeInstructionCreateDto(
   
    int? StepNumber,
    string? Description,
    string? Note
);
public record RecipeInstructionUpdateDto(
    int Id,
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
    string? Name,
    string? Description,
    string? Notes,
    MealType MealType,
    bool IsMultiDayMeal,
    int CreatedBy,
    DateTime? CreateAt,
    int UpdatedBy,
    DateTime? UpdatedAt

);



public record MealCreateDto(
    MealType MealType,
    string Name,
    string? Description,
    string? Notes,
    bool IsMultiDayMeal


);
public record MealUpdateDto(
    int Id,
    MealType MealType,
    string Name,
    string? Description,
    string? Notes,
    bool IsMultiDayMeal,

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