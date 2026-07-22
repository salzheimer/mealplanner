

using Rebus.Handlers;

namespace MealRecipeService.Consumers;
public class GroupChangedConsumer(Interfaces.ICachedService _cachedService): IHandleMessages<GroupChanged>
{
    public async Task Handle(GroupChanged msg)
    {
        
        if (msg.Action == "Deleted")
            await _cachedService.DeleteCachedGroup(msg.GroupId);
        else if(msg.Action =="Created")
            await _cachedService.AddCachedGroup(msg);
        else if(msg.Action =="Updated")
            await _cachedService.UpdateCachedGroup(msg);    
    }
}