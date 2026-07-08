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

[Table("cached_group_members")]
public class CachedGroupMember
{
    [Key]
    [Column("cached_group_member_id")]
    public Guid Id { get; set; }
    [Column("user_id")]
    public Guid UserId { get; set; }
    [Column("group_id")]
    public Guid GroupId { get; set; }
    [Column("role_id")]
    public int RoleId { get; set; }
    [Column("role_name")]
    public string RoleName { get; set; }
    [Column("status_id")]
    public int StatusId { get; set; }
    [Column("status_name")]
    public string StatusName { get; set; }
    [Column("synced_at")]
    public DateTimeOffset SyncedAt { get; set; }
}