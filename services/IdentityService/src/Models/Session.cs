using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace IdentityService.Models;

[Table("sessions")]
public class Session
{
    [Key]
    [Column("session_id")]
    public Guid Id { get; set; }= Guid.NewGuid();

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("token_hash")]
    public string TokenHash { get; set; } = string.Empty;

    [Column("client_type_id")]
    public int ClientTypeId { get; set; }

    [Column("device_info")]
    public string? DeviceInfo { get; set; }

    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [Column("last_used_at")]
    public DateTimeOffset? LastUsedAt { get; set; }

    [Column("revoked_at")]
    public DateTimeOffset? RevokedAt { get; set; }

    [Column("expires_at")]
    public DateTimeOffset ExpiresAt { get; set; }
}
