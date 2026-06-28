namespace MealRecipeService.Contracts;

//Recipe instructions DTOs
public record RecipeInstructionResponse(
    Guid Id,
    Guid RecipeId,
    int? StepNumber,
    string? Description,
    string? Note
);
public record CreateRecipeInstructionRequest(
    Guid RecipeId,
    int? StepNumber,
    string? Description,
    string? Note
);
public record UpdateRecipeInstructionRequest(
    Guid Id,
    int? StepNumber,
    string? Description,
    string? Note
);