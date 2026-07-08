using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models;

[Table("plan_meals")]
public class PlanMeal
{
    [Key]
    [Column("meal_id")]
    public Guid Id { get; set; }
    [Column("plan_id")]
    public Guid PlanId { get; set; }
    [Column("meal_id")]
    public Guid MealId { get; set; }
    [Column("serve_date")]
    public DateTime? ServeDate { get; set; }
    [Column("end_date")]
    public DateTime? EndDate { get; set; }
    [Column("added_by_user_id")]
    public Guid AddedByUserId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("updated_by")]
    public Guid UpdatedBy { get; set; }
}