using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace IdentityService.Models;

public static class GroupErrors
{
    public static readonly Error Unauthorized = new("Group.NotAuthorized", "Not authorize to add groups.", ErrorType.Unauthorized);
    public static readonly Error UnableToCreate = new("Group.UnableToCreate", "Unable to create group.", ErrorType.Failure);
    public static readonly Error UnableToDelete = new("Group.UnableToDelete", "Unable to delete group.", ErrorType.Failure);
    public static readonly Error NotFound = new("Group.NotFound", "Group not found.", ErrorType.NotFound);
    public static readonly Error UnableToUpdate = new("Group.UnableToUpdate", "Unable to update group.", ErrorType.Failure);
}

public static class GroupMemberErrors
{
    public static readonly Error UnableToCreate = new("GroupMember.UnableToCreate", "Unable to create group member.", ErrorType.Failure);
    public static readonly Error Unauthorized = new("GroupMember.NotAuthorized", "Not authorize to add group members.", ErrorType.Unauthorized);
    public static readonly Error UnableToLocate = new("GroupMember.UnableToLocate", "Unable to locate group member.", ErrorType.NotFound);
    public static readonly Error UnableToDelete = new("GroupMember.UnableToDelete", "Unable to delete group member.", ErrorType.Failure);
    public static readonly Error UnableToUpdate = new("GroupMember.UnableToUpdate", "Unable to update group member.", ErrorType.Failure);
}