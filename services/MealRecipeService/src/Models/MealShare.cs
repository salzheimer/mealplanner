namespace MealRecipeService.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("meal_share")]
public class MealShare
{
    [Key]
    public int Id { get; set; }
    [Column("meal_id")]
    public int MealId { get; set; }
    [Column("shared_with_user_id")]
    public int? SharedWithUserId { get; set; }
    [Column("shared_with_group_id")]
    public int? SharedWithGroupId { get; set; }
    [Column("shared_by_user_id")]
    public int SharedByUserId { get; set; }
    [Column("permission")]
    public Permission Permission { get; set; } = Permission.View;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

}

