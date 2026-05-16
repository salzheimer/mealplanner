using Shared.Models;

namespace IdentityService.Models;
public static class UserErrors
{
    public static readonly Error Unauthorized = new ("User.Unauthorized", "Invalid credentials.", ErrorType.Unauthorized);
    public static readonly Error NotFound = new ("User.NotFound", "Email not found.", ErrorType.NotFound);
    public static readonly Error UserAlreadyExists = new ("User.AlreadyExists", "User already exists.", ErrorType.BadRequest);
    public static readonly Error MissingEmailOrPassword = new ("User.MissingEmailOrPassword", "Email and password are required.", ErrorType.BadRequest);
    public static readonly Error InvalidRefreshToken = new ("User.InvalidRefreshToken", "The refresh token is invalid or has expired.", ErrorType.BadRequest);
}