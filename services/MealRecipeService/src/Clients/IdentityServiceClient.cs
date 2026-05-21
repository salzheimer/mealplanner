using System.Net.Http.Json;
using Shared.Models;
using Shared.Services;

namespace MealRecipeService.Clients;

public class IdentityServiceClient : IIdentityServiceClient
{
    private readonly string _baseUrl;
    private readonly string _serviceApiKey;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityServiceClient(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _baseUrl = configuration["IdentityService:BaseUrl"] ?? "http://auth-service";
        _serviceApiKey = configuration["ServiceApiKey"] ?? "";
        _httpContextAccessor = httpContextAccessor;
    }

    private string? GetBearerToken()
    {
        var header = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
        return header?.StartsWith("Bearer ") == true ? header["Bearer ".Length..] : null;
    }

    private HttpClient CreatePermissionClient()
    {
        var client = ServiceClient.CreateClient(_baseUrl, GetBearerToken());
        if (!string.IsNullOrWhiteSpace(_serviceApiKey))
            client.DefaultRequestHeaders.Add("X-Service-Key", _serviceApiKey);
        return client;
    }

    public async Task<Result<ResourcePermissionDto>> GrantPermissionAsync(ResourcePermissionCreateDto permission)
    {
        var client = CreatePermissionClient();
        var response = await client.PostAsJsonAsync("/api/permission", permission);
        if (!response.IsSuccessStatusCode)
            return Result<ResourcePermissionDto>.Failure(new Error("IdentityService.Error", "Failed to grant permission", ErrorType.BadRequest));
        var result = await response.Content.ReadFromJsonAsync<Result<ResourcePermissionDto>>();
        return result ?? Result<ResourcePermissionDto>.Failure(new Error("IdentityService.Error", "Empty response", ErrorType.BadRequest));
    }

    public async Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForResourceAsync(ResourceType resourceType, int resourceId)
    {
        var client = CreatePermissionClient();
        var response = await client.GetAsync($"/api/permission/resource-permissions?resourceType={resourceType}&resourceId={resourceId}");
        if (!response.IsSuccessStatusCode)
            return Result<IEnumerable<ResourcePermissionDto>>.Failure(new Error("IdentityService.Error", "Failed to get permissions", ErrorType.BadRequest));
        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<ResourcePermissionDto>>>();
        return result ?? Result<IEnumerable<ResourcePermissionDto>>.Failure(new Error("IdentityService.Error", "Empty response", ErrorType.BadRequest));
    }

    public async Task<Result<IEnumerable<ResourcePermissionDto>>> GetPermissionsForSubjectAsync(SubjectType subjectType, int subjectId)
    {
        var client = CreatePermissionClient();
        var response = await client.GetAsync($"/api/permission/subject-permissions?subjectType={subjectType}&subjectId={subjectId}");
        if (!response.IsSuccessStatusCode)
            return Result<IEnumerable<ResourcePermissionDto>>.Failure(new Error("IdentityService.Error", "Failed to get permissions", ErrorType.BadRequest));
        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<ResourcePermissionDto>>>();
        return result ?? Result<IEnumerable<ResourcePermissionDto>>.Failure(new Error("IdentityService.Error", "Empty response", ErrorType.BadRequest));
    }

    public async Task<Result<IEnumerable<ResourcePermissionDto>>> GetUserPermissionsAsync(int userId)
    {
        var client = CreatePermissionClient();
        var response = await client.GetAsync($"/api/permission/subject-permissions?subjectType=user&subjectId={userId}");
        if (!response.IsSuccessStatusCode)
            return Result<IEnumerable<ResourcePermissionDto>>.Failure(new Error("IdentityService.Error", "Failed to get permissions", ErrorType.BadRequest));
        var result = await response.Content.ReadFromJsonAsync<Result<IEnumerable<ResourcePermissionDto>>>();
        return result ?? Result<IEnumerable<ResourcePermissionDto>>.Failure(new Error("IdentityService.Error", "Empty response", ErrorType.BadRequest));
    }

    public async Task<Result<bool>> RevokePermissionAsync(long permissionId)
    {
        var client = CreatePermissionClient();
        var response = await client.DeleteAsync($"/api/permission/{permissionId}");
        if (!response.IsSuccessStatusCode)
            return Result<bool>.Failure(new Error("IdentityService.Error", "Failed to revoke permission", ErrorType.BadRequest));
        var result = await response.Content.ReadFromJsonAsync<Result<bool>>();
        return result ?? Result<bool>.Failure(new Error("IdentityService.Error", "Empty response", ErrorType.BadRequest));
    }
}
