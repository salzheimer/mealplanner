using PlanService.Models;
using SharedItemStatus = Shared.Models.ItemStatus;

namespace PlanService.Mappings;

internal static class EnumMappings
{
 public static SharedItemStatus ToDto(this ItemStatus status)
    {
        return status switch
        {
            ItemStatus.Unknown => SharedItemStatus.Unknown,
            ItemStatus.Pending => SharedItemStatus.Pending,
            ItemStatus.Confirmed => SharedItemStatus.Confirmed,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected item status value: {status}")
        };
    }
    public static ItemStatus ToEntity(this SharedItemStatus status)
    {
        return status switch
        {
            SharedItemStatus.Unknown => ItemStatus.Unknown,
            SharedItemStatus.Pending => ItemStatus.Pending,
            SharedItemStatus.Confirmed => ItemStatus.Confirmed,
            _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected item status value: {status}")
        };
    }
}