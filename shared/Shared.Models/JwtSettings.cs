namespace Shared.Models;

public record JwtSettings(
    string Issuer,
    string Audience,
    string Secret,
    int ExpiresMinutes
);
