using Shared.Models;

public static class CachedGroupErrors
{
    public static readonly Error UnableToCreate = new("CachedGroup.UnableToCreate", "Failed to create cached group.", ErrorType.Failure);
    public static readonly Error UnableToUpdate = new("CachedGroup.UnableToUpdate", "Failed to update cached group.", ErrorType.Failure);
    public static readonly Error UnableToDelete = new("CachedGroup.UnableToDelete", "Failed to delete cached group.", ErrorType.Failure);

}

public static class CachedGroupMembershipErrors
{
    public static readonly Error UnableToCreate = new("CachedGroupMembership.UnableToCreate", "Failed to create group member.", ErrorType.Failure);
    public static readonly Error UnableToUpdate = new("CachedGroupMembership.UnableToUpdate", "Failed to update group member.", ErrorType.Failure);
    public static readonly Error UnableToDelete = new("CachedGroupMembership.UnableToDelete", "Failed to delete group member.", ErrorType.Failure);

}
public static class CachedUserErrors
{
    public static readonly Error UnableToCreate = new("CachedUser.UnableToCreate", "Failed to create cached user.", ErrorType.Failure);
    public static readonly Error UnableToUpdate = new("CachedUser.UnableToUpdate", "Failed to update cached user.", ErrorType.Failure);
    public static readonly Error UnableToDelete = new("CachedUser.UnableToDelete", "Failed to delete cached user.", ErrorType.Failure);

}