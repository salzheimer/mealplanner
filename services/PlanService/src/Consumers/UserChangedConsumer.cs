

using Rebus.Handlers;

namespace PlanService.Consumers;
public class UserChangedConsumer(Interfaces.ICachedService _cachedService): IHandleMessages<UserChanged>
{
    public async Task Handle(UserChanged msg)
    {
        
        if (msg.Action == "Deleted")
            await _cachedService.DeleteCachedUser(msg.UserId);
        else if(msg.Action =="Created")
            await _cachedService.AddCachedUser(msg);
        else if(msg.Action =="Updated")
            await _cachedService.UpdateCachedUser(msg);    
    }
}