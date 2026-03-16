namespace AuthService.Models;

public record User
(
    int Id,
    string Username,
    string PasswordHash,
    string? Email
);
