using Shared.Models;
public static class ResourceTypeErrors
{
    public static readonly Error UnableToCreate = new("ResourceType.UnableToCreate", "Unable to create Resource Type", ErrorType.BadRequest);
    public static readonly Error InvalidType = new("ResourceType.InvalidType", "Unable to locate Resource Type", ErrorType.NotFound);
    public static readonly Error Unauthorized = new("ResourceType.Unauthorized", "You do not have permission to access resource types.", ErrorType.Unauthorized);
}
public static class PermissionTypeErrors
{
    public static readonly Error UnableToCreate = new("PermissionType.UnableToCreate", "Unable to create Permission Type", ErrorType.BadRequest);
    public static readonly Error InvalidType = new("PermissionType.InvalidType", "Unable to locate Permission Type", ErrorType.NotFound);
    public static readonly Error Unauthorized = new("PermissionType.Unauthorized", "You do not have permission to access permission types.", ErrorType.Unauthorized);
}
public static class SubjectTypeErrors
{
    public static readonly Error UnableToCreate = new("SubjectType.UnableToCreate", "Unable to create Subject Type", ErrorType.BadRequest);
    public static readonly Error InvalidType = new("SubjectType.InvalidType", "Unable to locate Subject Type", ErrorType.NotFound);
    public static readonly Error Unauthorized = new("SubjectType.Unauthorized", "You do not have permission to access subject types.", ErrorType.Unauthorized);
}
public static class ResourcePermissionErrors
{
    public static readonly Error UnableToCreate = new("ResourcePermission.UnableToCreate", "Unable to grant access to resource", ErrorType.BadRequest);
    public static readonly Error InvalidType = new("ResourcePermission.InvalidType", "Unable to locate resource permissions", ErrorType.NotFound);
    public static readonly Error Unauthorized = new("ResourcePermission.Unauthorized", "You do not have permission to access.", ErrorType.Unauthorized);
    public static readonly Error UnableToDelete = new("ResourcePermission.UnableToDelete", "Failed to revoke access.", ErrorType.BadRequest);
}