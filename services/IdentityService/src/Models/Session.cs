using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace IdentityService.Models;

[Table("session")]
public class Session
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("token_hash")]
    public string TokenHash { get; set; } = string.Empty;

    [Column("client_type")]
    public ClientType ClientType { get; set; } = ClientType.Api;

    [Column("device_info")]
    public string? DeviceInfo { get; set; }

    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("last_used_at")]
    public DateTime? LastUsedAt { get; set; }

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }
}
