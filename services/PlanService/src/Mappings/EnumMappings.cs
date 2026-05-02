using PlanService.Models;
using SharedItemStatus = Shared.Models.ItemStatus;
using SharedPermission = Shared.Models.Permission;

namespace PlanService.Mappings;

internal static class EnumMappings
{
 public static SharedItemStatus ToDtoItemStatus(this ItemStatus status)
    {
        return status switch
        {
            ItemStatus.Unknown => SharedItemStatus.Unknown,
            ItemStatus.Pending => SharedItemStatus.Pending,
            ItemStatus.Confirmed => SharedItemStatus.Confirmed,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected item status value: {status}")
        };
    }
    public static ItemStatus ToEntityItemStatus(this SharedItemStatus status)
    {
        return status switch
        {
            SharedItemStatus.Unknown => ItemStatus.Unknown,
            SharedItemStatus.Pending => ItemStatus.Pending,
            SharedItemStatus.Confirmed => ItemStatus.Confirmed,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected item status value: {status}")
        };
    }
     public static SharedPermission ToDtoPermission(this Permission permission)
    {
        return permission switch
        {
            Permission.View => SharedPermission.View,
            Permission.Edit => SharedPermission.Edit,
            _ => throw new ArgumentOutOfRangeException(nameof(permission), $"Not expected permission value: {permission}")
        };
    }
    public static Permission ToEntityPermission(this SharedPermission permission)
    {
        return permission switch
        {
            SharedPermission.View => Permission.View,
            SharedPermission.Edit => Permission.Edit,
            _ => throw new ArgumentOutOfRangeException(nameof(permission), $"Not expected permission value: {permission}")
        };
    }
}