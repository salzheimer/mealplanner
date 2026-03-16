namespace Shared.Models;

public record UserDto(
    int Id,
    string Username,
    string Email
);

public record RegisterRequest(
    string Username,
    string Password,
    string? Email
);

public record LoginRequest(
    string Username,
    string Password
);

public record LoginResponse(
    string AccessToken,
    string TokenType = "Bearer",
    int ExpiresInSeconds = 3600
);
