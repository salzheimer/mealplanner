using Shared.Services;

namespace GatewayBff.Services;

/// <summary>
/// Thin seam around Shared.Services.ServiceClient so DashboardService can be unit
/// tested with a substituted HttpClient (mocked HttpMessageHandler) instead of a
/// real socket connection. The default implementation just delegates — production
/// behavior is identical to calling ServiceClient.CreateClient directly.
/// </summary>
public interface IServiceClientFactory
{
    HttpClient CreateClient(string baseAddress, string? bearerToken);
}

public class ServiceClientFactory : IServiceClientFactory
{
    public HttpClient CreateClient(string baseAddress, string? bearerToken) =>
        ServiceClient.CreateClient(baseAddress, bearerToken);
}
