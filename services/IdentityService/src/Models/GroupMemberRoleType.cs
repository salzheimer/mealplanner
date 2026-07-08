using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models;

[Table("group_member_role_types")]
public class GroupMemberRoleType
{
    [Column("group_member_role_id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;
    [Column("sort_order")]
    public int SortOrder { get; set; }
}
