using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models;

[Table("plan_share")]
public class MealPlan
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("plan_id")]
    public int PlanId { get; set; }
    [Column("meal_id")]
    public int MealId { get; set; }
    [Column("serve_date")]
    public DateTime? ServeDate { get; set; }
    [Column("end_date")]
    public DateTime? EndDate { get; set; }
    [Column("added_by_user_id")]
    public int AddedByUserId { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("created_by")]
    public int CreatedBy { get; set; }
    [Column("updated_by")]
    public int UpdatedBy { get; set; }
}