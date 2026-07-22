
using IdentityService.Interfaces;
using Rebus.Bus;
using Rebus.Handlers;
namespace IdentityService.Consumers;

public class CacheSyncRequestedHandler(IUserService userService, IGroupService groupService,
   IBus bus) : IHandleMessages<CacheSyncRequested>
{
    public async Task Handle(CacheSyncRequested message)
    {
        
        var users = await userService.GetAllUsersAsync();
        if (users.IsSuccess)
        {
            foreach (var user in users.Value!)
                await bus.Publish(new UserChanged { UserId = user.Id, DisplayName = user.DisplayName, Action = "Created" });
        }
        var groups = await groupService.GetAllGroupsAsync();
        if (groups.IsSuccess)
        {
            foreach (var group in groups.Value!)
                await bus.Publish(new GroupChanged { GroupId = group.GroupId, GroupName = group.Name, Action = "Created" });
        }
        // if group member dependencies are successful and has values
        if (users.IsSuccess && groups.IsSuccess && (users.Value?.Any() ?? false) && (groups.Value?.Any() ?? false))
        {
            var groupMembers = await groupService.GetAllGroupMembersAsync();
            if(groupMembers.IsSuccess)
            foreach (var member in groupMembers.Value!)
                await bus.Publish(new GroupMembershipChanged { UserId = member.UserId, GroupId = member.GroupId, RoleName = member.GroupMemberRoleTypeName, Action = "Added" });
        }
    }
}