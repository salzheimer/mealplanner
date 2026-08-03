using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ApiGateway.Tests;

/// <summary>
/// Verifies YARP route matching from appsettings.json's ReverseProxy section.
/// Cluster destinations are overridden to an unreachable port so a match produces
/// a 502 (proxy attempted, no downstream listening) rather than requiring live
/// backend services; an unmatched path produces 404 from ASP.NET Core routing
/// before YARP is ever involved. This distinguishes "route matched" from
/// "route didn't match" without standing up the real microservices.
/// </summary>
public class ReverseProxyRoutingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string UnreachableAddress = "http://127.0.0.1:1";
    private readonly HttpClient _client;

    public ReverseProxyRoutingTests(WebApplicationFactory<Program> factory)
    {
        var configuredFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ReverseProxy:Clusters:identity:Destinations:d1:Address"] = UnreachableAddress,
                    ["ReverseProxy:Clusters:meal:Destinations:d1:Address"] = UnreachableAddress,
                    ["ReverseProxy:Clusters:plan:Destinations:d1:Address"] = UnreachableAddress,
                    ["ReverseProxy:Clusters:bff:Destinations:d1:Address"] = UnreachableAddress,
                });
            });
        });

        _client = configuredFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Theory]
    [InlineData("/api/auth/login")]
    [InlineData("/api/recipes/1")]
    [InlineData("/api/meal/1")]
    [InlineData("/api/plans/1")]
    [InlineData("/api/mealplan/1")]
    [InlineData("/graphql")]
    [InlineData("/graphql/ws")]
    public async Task MatchedRoute_ProxiesToCluster_AndFailsWithBadGateway(string path)
    {
        var response = await _client.GetAsync(path);

        // 502 proves YARP matched a route and attempted to proxy — it never gets this far
        // for a path with no matching route (see UnmatchedRoute_ReturnsNotFound).
        Assert.Equal(System.Net.HttpStatusCode.BadGateway, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/unknown/thing")]
    [InlineData("/completely/random")]
    [InlineData("/api")]
    public async Task UnmatchedRoute_ReturnsNotFound(string path)
    {
        var response = await _client.GetAsync(path);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GatewayOwnController_IsNotShadowedByReverseProxy()
    {
        var response = await _client.GetAsync("/api/gateway/status");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
    }
}
