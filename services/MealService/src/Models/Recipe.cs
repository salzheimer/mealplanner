public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public int? Ranking { get; set; }
    public string? OriginalSource { get; set; }
    public TimeSpan? CookTime { get; set; }
    public TimeSpan? PrepTime { get; set; }
    public int? Servings { get; set; }
    public int? OwnerUserId { get; set; }
    public Visibility? Visibility { get; set; } = global::Visibility.Private;
    public DateTime CreatedAt { get; set; }
    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public List<RecipeInstruction> Instructions { get; set; } = new();
}

public enum Visibility
{
    Private,
    Shared,
    Group
}