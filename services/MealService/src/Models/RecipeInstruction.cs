public class RecipeInstruction
{
    public int Id { get; set; }
    public int? RecipeId { get; set; }
    public int? StepNumber { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
}