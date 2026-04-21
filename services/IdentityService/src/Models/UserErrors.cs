using Shared.Models;

namespace IdentityService.Models;
public static class UserErrors
{
    public static readonly Error Unauthorized = new ("User.Unauthorized", "Invalid credentials.");
    public static readonly Error NotFound = new ("User.NotFound", "Email not found.");
    public static readonly Error UserAlreadyExists = new ("User.AlreadyExists", "User already exists.");
    public static readonly Error MissingEmailOrPassword = new ("User.MissingEmailOrPassword", "Email and password are required.");
}