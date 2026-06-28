namespace MealRecipeService.Contracts;


public record MealItemResponse(
    Guid Id,
    string Name,
    Guid? RecipeId,
    Guid MealId,
    int ItemTypeId,
    string ItemTypeName
);

public record MealItemRequest(
    Guid Id,
    string Name,
    Guid? RecipeId,
    Guid MealId,
    int ItemTypeId,
    string ItemTypeName,
    Guid CreatedBy
);
public record CreateMealItemRequest(
    string Name,
    Guid? RecipeId,
    Guid MealId,
    int ItemTypeId,
    string ItemTypeName,
    Guid CreatedBy
);
public record UpdateMealItemRequest(
    Guid Id,
    string Name,
    Guid? RecipeId,
    Guid MealId,
    int ItemTypeId,
    string ItemTypeName,
    Guid UpdatedBy
);
public record MealItemDetailResponse(
    Guid Id,
    string Name,
    Guid? RecipeId,
    Guid MealId,
    int ItemTypeId,
    string ItemTypeName,
    Guid CreatedBy,
    Guid UpdatedBy,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);