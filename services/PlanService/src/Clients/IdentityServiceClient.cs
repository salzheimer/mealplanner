using System.Net.Http.Json;
using Shared.Models;
using Shared.Services;

namespace PlanService.Clients;

public class IdentityServiceClient : IIdentityServiceClient
{
    private readonly string _baseUrl;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityServiceClient(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _baseUrl = configuration["IdentityService:BaseUrl"] ?? "http://auth-service";
        _httpContextAccessor = httpContextAccessor;
    }

    private string? GetBearerToken()
    {
        var header = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        return header?.StartsWith("Bearer ") == true ? header["Bearer ".Length..] : null;
    }

    public async Task<Result<ResourcePermissionDto>> GrantPermissionAsync(ResourcePermissionCreateDto permission)
    {
        var client = ServiceClient.CreateClient(_baseUrl, GetBearerToken());
        var response = await client.PostAsJsonAsync("/api/auth/grant-permission", permission);
        if (!response.IsSuccessStatusCode)
            return Result<ResourcePermissionDto>.Failure(new Error("IdentityService.Error", "Failed to grant permission"));
        var result = await response.Content.ReadFromJsonAsync<Result<ResourcePermissionDto>>();
        return result ?? Result<ResourcePermissionDto>.Failure(new Error("IdentityService.Error", "Empty response"));
    }

    public async Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForResourceAsync(ResourceType resourceType, int resourceId)
    {
        var client = ServiceClient.CreateClient(_baseUrl, GetBearerToken());
        var response = await client.GetAsync($"/api/auth/resource-permissions?resourceType={resourceType}&resourceId={resourceId}");
        if (!response.IsSuccessStatusCode)
            return Result<IEnumerable<ResourcePermissionDto>>.Failure(new Error("IdentityService.Error", "Failed to get permissions"));
        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<ResourcePermissionDto>>>();
        return result ?? Result<IEnumerable<ResourcePermissionDto>>.Failure(new Error("IdentityService.Error", "Empty response"));
    }

    public async Task<Result<bool>> RevokePermissionAsync(long permissionId)
    {
        var client = ServiceClient.CreateClient(_baseUrl, GetBearerToken());
        var response = await client.DeleteAsync($"/api/auth/permissions/{permissionId}");
        if (!response.IsSuccessStatusCode)
            return Result<bool>.Failure(new Error("IdentityService.Error", "Failed to revoke permission"));
        var result = await response.Content.ReadFromJsonAsync<Result<bool>>();
        return result ?? Result<bool>.Failure(new Error("IdentityService.Error", "Empty response"));
    }
}
