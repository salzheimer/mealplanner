using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MealRecipeService.Models;
[Table("cached_users")]
public class CachedUser
{
    [Key]
    [Column("cached_user_id")]
    public Guid Id { get; set; }
    [Column("display_name")]
    public string DisplayName { get; set; } = null!;
    [Column("synced_at")]
    public DateTimeOffset SyncedAt { get; set; }
}

[Table("cached_groups")]
public class CachedGroup
{
    [Key]
    [Column("cached_group_id")]
    public Guid Id { get; set; }
    [Column("display_name")]
    public string DisplayName { get; set; } = null!;
    [Column("synced_at")]
    public DateTimeOffset SyncedAt { get; set; }
}