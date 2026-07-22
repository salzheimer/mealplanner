namespace IdentityService.Contracts;
public record UpdateUserProfileRequest(
    Guid Id,
    string DisplayName
);
 
public record CreateUserRequest(
    string Email,
    string Password,
    string? DisplayName
    
);
public record UserResponse(
    Guid Id,
    string Email,
    string DisplayName
);

public record UpdateUserCredentialsRequest(
    Guid UserId,
    string Password
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