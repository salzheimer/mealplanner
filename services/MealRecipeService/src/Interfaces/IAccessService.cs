using MealRecipeService.Contracts;
using MealRecipeService.Models;
using Shared.Models;

namespace MealRecipeService.Interfaces;

public interface IAccessService
{
    Task<Result<ResourcePermissionResponse>> GrantGroupPermissionToResource(GroupGrantAccessRequest groupGrantAccessRequest);
    Task<Result<ResourcePermissionResponse>> GrantUserPermissionToResource(UserGrantAccessRequest userGrantAccessRequest);
    Task<Result<bool>> RevokeUserPermissionToResource(UserRevokeAccessRequest revokeAccessRequest);
    Task<Result<bool>> RevokeGroupPermissionToResource(GroupRevokeAccessRequest revokeAccessRequest);
    Task<Result<ResourcePermissionResponse>> GrantAccessToResource(CreateResourcePermissionRequest request);
    Task<Result<IEnumerable<ResourcePermissionResponse>>> GetSharedRecipes(Guid recipeId);
    Task<Result<IEnumerable<ResourcePermissionResponse>>> GetSharedMeals(Guid mealId);
    Task<Result<IEnumerable<ResourcePermissionResponse>>> GetResourcesSharedWithUser(Guid userId);
    Task<Result<IEnumerable<ResourcePermissionResponse>>> GetResourcesSharedWithGroup(Guid groupId);
    Task<Result<IEnumerable<ResourcePermissionResponse>>> GetResourcesSharedByUser(Guid grantedById);
    Task<Result<IEnumerable<ResourcePermissionResponse>>> GetRecipesSharedWithUser(Guid userId);
    Task<Result<IEnumerable<ResourcePermissionResponse>>> GetMealsSharedWithUser(Guid userId);

}