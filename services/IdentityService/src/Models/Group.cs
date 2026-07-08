using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityService.Models;

[Table("groups")]
public class Group
{
    [Key]
    [Column("group_id")]
    public Guid GroupId{get;set;}
    [Column("name")]
    public string Name {get;set;} = string.Empty;
    [Column("created_by_user_id")]
    public Guid CreatedBy{get;set;}
    [Column("created_at")]
    public DateTimeOffset CreatedAt{get;set;}

    public ICollection<GroupMember> GroupMembers {get;set;}=[];
}