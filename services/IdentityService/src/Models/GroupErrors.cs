using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace IdentityService.Models;

public static class GroupErrors
{
    public static readonly Error UnableToCreate = new("Group.UnableToCreate", "Unable to create group.", ErrorType.Failure);
    public static readonly Error UnableToDelete = new("Group.UnableToDelete", "Unable to delete group.", ErrorType.Failure);
    public static readonly Error NotFound = new("Group.NotFound", "Group not found.", ErrorType.NotFound);
    public static readonly Error UserAlreadyExists = new("User.AlreadyExists", "User already exists.", ErrorType.BadRequest);
    public static readonly Error NotAValidEmailAddress = new("User.NotAValidEmailAddress", "Not a recognized Email address", ErrorType.InvalidInput);
    public static readonly Error UserPasswordValidationFailed = new("User.PasswordValidation", "Password failed security validation", ErrorType.Failure);
    public static readonly Error MissingEmailOrPassword = new("User.MissingEmailOrPassword", "Email and password are required.", ErrorType.BadRequest);
    public static readonly Error InvalidRefreshToken = new("User.InvalidRefreshToken", "The refresh token is invalid or has expired.", ErrorType.BadRequest);
    public static readonly Error Unauthorized = new("Group.NotAuthorized", "Not authorize to add a group.", ErrorType.Unauthorized);
}

public static class GroupMemberErrors
{
    public static readonly Error UnableToCreate = new("GroupMember.UnableToCreate", "Unable to create group member.", ErrorType.Failure);
    public static readonly Error Unauthorized = new("GroupMember.NotAuthorized", "Not authorize to add group members.", ErrorType.Unauthorized);
    public static readonly Error UnableToLocate = new("GroupMember.UnableToLocate", "Unable to locate group member.", ErrorType.NotFound);
}