using System.Net.Http.Headers;

namespace Shared.Services;

public static class ServiceClient
{
    public static HttpClient CreateClient(string baseAddress, string? bearerToken = null)
    {
        var client = new HttpClient { BaseAddress = new Uri(baseAddress) };
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        return client;
    }
}
