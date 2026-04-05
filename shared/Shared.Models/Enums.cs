namespace Shared.Models;

public enum Visibility
{
    Private,
    Shared,
    Group
}

public enum MealType
{
    Breakfast,
    Lunch,
    Dinner,
    Snack
}

public enum ItemType
{
    Recipe,
    Homemade,
    StoreBought
}

public enum ItemStatus
{
    Unknown,
    Pending,
    Confirmed
}

public enum GroupMemberRole
{
    Owner,
    Member
}

public enum GroupMemberStatus
{
    Pending,
    Active,
    Removed
}

public enum Permission
{
    View,
    Edit
}

public enum ClientType
{
    Web,
    Mobile,
    Api
}
