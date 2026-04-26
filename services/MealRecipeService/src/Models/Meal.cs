using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MealRecipeService.Models;

namespace MealRecipeService.Models;

[Table("meal")]
public class Meal
{
    [Key]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;
    [Column("description")]
    public string? Description { get; set; }
    [Column("notes")]
    public string? Notes { get; set; }
    [Column("meal_type")]
    public MealType MealType { get; set; }
    [Column("is_multi_day_meal")]
    public bool? IsMultiDayMeal { get; set; }
    [Column("visibility")]
    public Visibility Visibility { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}


