using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace IdentityService.Models;
public static class 

UserErrors
{
    public static readonly Error Unauthorized = new ("User.Unauthorized", "Invalid credentials.", ErrorType.Unauthorized);
    public static readonly Error NotFound = new ("User.NotFound", "Email not found.", ErrorType.NotFound);
    public static readonly Error UserAlreadyExists = new ("User.AlreadyExists", "User already exists.", ErrorType.BadRequest);
    public static readonly Error NotAValidEmailAddress = new ("User.NotAValidEmailAddress", "Not a recognized Email address", ErrorType.InvalidInput);
    public static readonly Error UserPasswordValidationFailed = new ("User.PasswordValidation", "Password failed security validation", ErrorType.Failure);
    public static readonly Error MissingEmailOrPassword = new ("User.MissingEmailOrPassword", "Email and password are required.", ErrorType.BadRequest);
    public static readonly Error InvalidRefreshToken = new ("User.InvalidRefreshToken", "The refresh token is invalid or has expired.", ErrorType.BadRequest);
    public static readonly Error UpdateFailed = new ("User.UpdateFailed", "User Update failed.", ErrorType.Failure);
     public static readonly Error DeleteFailed = new ("User.DeleteFailed", "User Delete failed.", ErrorType.Failure);
}