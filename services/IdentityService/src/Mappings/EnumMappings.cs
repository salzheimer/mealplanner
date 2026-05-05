using IdentityService.Models;
using SharedPermission = Shared.Models.Permission;
using SharedResourceType = Shared.Models.ResourceType;
using SharedSubjectType = Shared.Models.SubjectType;

namespace IdentityService.Mappings;

public static class EnumMappings
{
    public static SharedPermission ToDtoPermission(this Permission permission)
    {
        return permission switch
        {
            Permission.View => SharedPermission.View,
            Permission.Edit => SharedPermission.Edit,
            Permission.Comment => SharedPermission.Comment,
            Permission.Manage => SharedPermission.Manage,
            _ => throw new ArgumentOutOfRangeException(nameof(permission), $"Not expected permission value: {permission}")
        };
    }
    public static Permission ToEntityPermission(this SharedPermission permission)
    {
        return permission switch
        {
            SharedPermission.View => Permission.View,
            SharedPermission.Edit => Permission.Edit,
            SharedPermission.Comment => Permission.Comment,
            SharedPermission.Manage => Permission.Manage,
            _ => throw new ArgumentOutOfRangeException(nameof(permission), $"Not expected permission value: {permission}")
        };
    }
    public static SharedResourceType ToDtoResourceType(this ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Recipe => SharedResourceType.Recipe,
            ResourceType.Meal => SharedResourceType.Meal,
            ResourceType.Plan => SharedResourceType.Plan,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType), $"Not expected resource type value: {resourceType}")
        };
    }
    public static ResourceType ToEntityResourceType(this SharedResourceType resourceType)
    {
        return resourceType switch
        {
            SharedResourceType.Recipe => ResourceType.Recipe,
            SharedResourceType.Meal => ResourceType.Meal,
            SharedResourceType.Plan => ResourceType.Plan,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType), $"Not expected resource type value: {resourceType}")
        };
    }
    public static SharedSubjectType ToDtoSubjectType(this SubjectType subjectType)
    {
        return subjectType switch
        {
            SubjectType.User => SharedSubjectType.User,
            SubjectType.Group => SharedSubjectType.Group,
            _ => throw new ArgumentOutOfRangeException(nameof(subjectType), $"Not expected subject type value: {subjectType}")
        };
    }
    public static SubjectType ToEntitySubjectType(this SharedSubjectType subjectType)
    {
        return subjectType switch
        {
            SharedSubjectType.User => SubjectType.User,
            SharedSubjectType.Group => SubjectType.Group,
            _ => throw new ArgumentOutOfRangeException(nameof(subjectType), $"Not expected subject type value: {subjectType}")
        };
    }
    public static Shared.Models.ClientType ToDtoClientType(this ClientType clientType)
    {
        return clientType switch
        {
            ClientType.Web => Shared.Models.ClientType.Web,
            ClientType.Mobile => Shared.Models.ClientType.Mobile,
            ClientType.Api => Shared.Models.ClientType.Api,
            _ => throw new ArgumentOutOfRangeException(nameof(clientType), $"Not expected client type value: {clientType}")
        };
    }
    public static ClientType ToEntityClientType(this Shared.Models.ClientType clientType)
    {
        return clientType switch
        {
            Shared.Models.ClientType.Web => ClientType.Web,
            Shared.Models.ClientType.Mobile => ClientType.Mobile,
            Shared.Models.ClientType.Api => ClientType.Api,
            _ => throw new ArgumentOutOfRangeException(nameof(clientType), $"Not expected client type value: {clientType}")
        };
    }
}