using Shared.Models;

namespace IdentityService.Models;

public static class PermissionErrors
{
    public static readonly Error Unauthorized = new("Permission.UnAuthorized", "User not authorized", ErrorType.Unauthorized);
    public static readonly Error NotFound = new("Permission.NotFound", "Permission not found", ErrorType.NotFound);

    public static readonly Error InvalidResourceType = new("Permission.InvalidResourceType", "Resource Type not found", ErrorType.InvalidInput);
    public static readonly Error InvalidSubjectType = new("Permission.InvalidSubjectType", "Subject Type not found", ErrorType.InvalidInput);
}