using MealRecipeService.Models;
using SharedMealType = Shared.Models.MealType;
using SharedItemType = Shared.Models.ItemType;

namespace MealRecipeService.Mappings;

internal static class EnumMappings
{

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