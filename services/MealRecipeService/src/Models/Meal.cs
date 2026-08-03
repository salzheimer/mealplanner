using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MealRecipeService.Models;

namespace MealRecipeService.Models;

[Table("meals")]
public class Meal
{
    [Key]
    [Column("meal_id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = null!;
    [Column("description")]
    public string? Description { get; set; }
    [Column("notes")]
    public string? Notes { get; set; }
    [Column("meal_type_id")]
    public int MealTypeId { get; set; }
    [Column("is_multi_day_meal")]
    public bool IsMultiDayMeal { get; set; }
    [Column("created_by")]
    public Guid CreatedBy {get;set;}
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_by")]
    public Guid UpdatedBy {get;set;}
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("owner_user_id")]
    public Guid OwnerUserId { get; set; }

    public MealType MealType { get; set; } = null!;
}


