namespace Shared.Models;

public record UserDto(
    Guid Id,
    string DisplayName,
    string Email
);
public record CreateUserDto(
    string Email,
    string Password,
    string DisplayName
    
);
public record UserResponseDto(
    Guid Id,
    string Email,
    string DisplayName
);
public record RegisterRequest(
    
    string Email,
    string Password,
    string? DisplayName = null
);
public record LoginRequest(
    string Email,
    string Password
);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    string TokenType = "Bearer",
    int ExpiresInSeconds = 900
);

public record RefreshRequest(string RefreshToken);
