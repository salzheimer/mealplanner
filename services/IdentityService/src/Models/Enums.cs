
namespace   IdentityService.Models;


public enum SubjectType
{
    User,
    Group
}
public enum Permission
{
    View,
    Edit,
    Comment,
    Manage
}
public enum ClientType
{
    Web,
    Mobile,
    Api
}
public enum ResourceType
{
    Recipe,
    Meal,
    Plan    
}