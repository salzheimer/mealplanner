using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanService.Models;

[Table("plan")]
public class Plan
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public string? Name { get; set; } = string.Empty;
    [Column("start_date")]
    public DateTime StartDate { get; set; }
    [Column("end_date")]
    public DateTime? EndDate { get; set; }
    [Column("owner_user_id")]
    public int OwnerUserId { get; set; }
}