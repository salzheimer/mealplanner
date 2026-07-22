

using Rebus.Handlers;

namespace MealRecipeService.Consumers;
public class GroupMemberChangedConsumer(Interfaces.ICachedService _cachedService): IHandleMessages<GroupMembershipChanged>
{
    public async Task Handle(GroupMembershipChanged msg)
    {
        
        if (msg.Action == "Deleted")
            await _cachedService.DeleteCachedGroupMember(msg.GroupMemberId);
        else if(msg.Action =="Created")
            await _cachedService.AddCachedGroupMember(msg);
        else if(msg.Action =="Updated")
            await _cachedService.UpdateCachedGroupMember(msg);    
    }
}