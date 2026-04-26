using MealRecipeService.Models;
using SharedVisibility = Shared.Models.Visibility;
using SharedPermission = Shared.Models.Permission;
using SharedItemStatus = Shared.Models.ItemStatus;
using SharedMealType = Shared.Models.MealType;
using SharedItemType = Shared.Models.ItemType;

using SharedMealItemType = Shared.Models.ItemType;

namespace MealRecipeService.Mappings;

internal static class EnumMappings
{
    public static SharedVisibility ToDtoVisibility(this Visibility visibility)
    {
        return visibility switch
        {
            Visibility.Private => SharedVisibility.Private,
            Visibility.Shared => SharedVisibility.Shared,
            Visibility.Group => SharedVisibility.Group,
            _ => throw new ArgumentOutOfRangeException(nameof(visibility), $"Not expected visibility value: {visibility}")
        };
    }

    public static Visibility ToEntityVisibility(this SharedVisibility visibility)
    {
        return visibility switch
        {
            SharedVisibility.Private => Visibility.Private,
            SharedVisibility.Shared => Visibility.Shared,
            SharedVisibility.Group => Visibility.Group,
            _ => throw new ArgumentOutOfRangeException(nameof(visibility), $"Not expected visibility value: {visibility}")
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
   
    public static SharedMealType ToDtoMealType(this MealType mealType)
    {        return mealType switch
        {
            MealType.Breakfast => SharedMealType.Breakfast,
            MealType.Lunch => SharedMealType.Lunch,
            MealType.Dinner => SharedMealType.Dinner,
            MealType.Snack => SharedMealType.Snack,
            _ => throw new ArgumentOutOfRangeException(nameof(mealType), $"Not expected meal type value: {mealType}")
        };     
    }
    public static MealType ToEntityMealType(SharedMealType mealType)
    {
        return mealType switch
        {
            SharedMealType.Breakfast => MealType.Breakfast,
            SharedMealType.Lunch => MealType.Lunch,
            SharedMealType.Dinner => MealType.Dinner,
            SharedMealType.Snack => MealType.Snack,
            _ => throw new ArgumentOutOfRangeException(nameof(mealType), $"Not expected meal type value: {mealType}")
        };
    }
    public static SharedItemType ToDtoItemType(this ItemType itemType)
    {        return itemType switch
        {
            ItemType.Recipe => SharedItemType.Recipe,
            ItemType.Homemade => SharedItemType.Homemade,
            ItemType.StoreBought => SharedItemType.StoreBought,
            _ => throw new ArgumentOutOfRangeException(nameof(itemType), $"Not expected item type value: {itemType}")
        };  
    }
    public static ItemType ToEntityItemType(this SharedItemType itemType)
    {
        return itemType switch
        {
            SharedItemType.Recipe => ItemType.Recipe,
            SharedItemType.Homemade => ItemType.Homemade,
            SharedItemType.StoreBought => ItemType.StoreBought,
            _ => throw new ArgumentOutOfRangeException(nameof(itemType), $"Not expected item type value: {itemType}")
        };
    } 
   
   

    }