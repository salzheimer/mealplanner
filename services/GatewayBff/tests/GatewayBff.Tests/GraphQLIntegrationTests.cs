using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GatewayBff.Services;
using GatewayBff.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;

namespace GatewayBff.Tests;

/// <summary>
/// End-to-end tests through the real GraphQL pipeline (WebApplicationFactory + real
/// HTTP POST to /graphql), with IDashboardService substituted so no real PlanService
/// or MealRecipeService is required. Covers the three behaviors the checklist calls
/// for: successful composition, missing-auth surfaces a GraphQL error (not silent
/// empty data), and a partial upstream failure surfaces a field-level error alongside
/// the data that did succeed.
/// </summary>
public class GraphQLIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Query = """
        query($start: DateTime!, $end: DateTime!) {
          dashboardMealPlans(startDate: $start, endDate: $end) {
            id
            mealId
            meal { name }
          }
        }
        """;

    private readonly WebApplicationFactory<Program> _factory;

    public GraphQLIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClientWithMockedDashboard(Mock<IDashboardService> dashboardServiceMock)
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDashboardService>();
                services.AddScoped(_ => dashboardServiceMock.Object);
            });
        });

        return factory.CreateClient();
    }

    private static async Task<JsonDocument> PostGraphQL(HttpClient client, string? bearerToken)
    {
        var payload = JsonSerializer.Serialize(new
        {
            query = Query,
            variables = new { start = "2026-08-01T00:00:00Z", end = "2026-08-08T00:00:00Z" }
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, "/graphql")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (bearerToken is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body);
    }

    [Fact]
    public async Task Query_WithValidToken_ReturnsComposedData()
    {
        var mealPlanId = Guid.NewGuid();
        var dashboardServiceMock = new Mock<IDashboardService>();
        dashboardServiceMock
            .Setup(s => s.GetDashboardMealPlansAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "good-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DashboardResult(
                [new DashboardMealPlan(mealPlanId, Guid.NewGuid(), Guid.NewGuid(), null, null,
                    new MealSummary(Guid.NewGuid(), "Monday Dinner", null, 1, false, Guid.NewGuid()))],
                PartialFailureMessage: null));

        var client = CreateClientWithMockedDashboard(dashboardServiceMock);
        var json = await PostGraphQL(client, "good-token");

        Assert.False(json.RootElement.TryGetProperty("errors", out _));
        var mealPlans = json.RootElement.GetProperty("data").GetProperty("dashboardMealPlans");
        Assert.Equal(1, mealPlans.GetArrayLength());
        Assert.Equal(mealPlanId.ToString(), mealPlans[0].GetProperty("id").GetString());
        Assert.Equal("Monday Dinner", mealPlans[0].GetProperty("meal").GetProperty("name").GetString());
    }

    [Fact]
    public async Task Query_WithoutAuthorizationHeader_ReturnsGraphQLErrorNotEmptyData()
    {
        var dashboardServiceMock = new Mock<IDashboardService>();
        var client = CreateClientWithMockedDashboard(dashboardServiceMock);

        var json = await PostGraphQL(client, bearerToken: null);

        Assert.True(json.RootElement.TryGetProperty("errors", out var errors));
        Assert.True(errors.GetArrayLength() > 0);
        Assert.Contains("Authorization", errors[0].GetProperty("message").GetString());

        // The resolver must never have been reached with no credentials.
        dashboardServiceMock.Verify(
            s => s.GetDashboardMealPlansAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Query_UpstreamPartialFailure_ReturnsDataAndFieldLevelError()
    {
        var mealPlanId = Guid.NewGuid();
        var dashboardServiceMock = new Mock<IDashboardService>();
        dashboardServiceMock
            .Setup(s => s.GetDashboardMealPlansAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "good-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DashboardResult(
                [new DashboardMealPlan(mealPlanId, Guid.NewGuid(), Guid.NewGuid(), null, null, Meal: null)],
                PartialFailureMessage: "MealRecipeService returned HTTP 503; meal details omitted."));

        var client = CreateClientWithMockedDashboard(dashboardServiceMock);
        var json = await PostGraphQL(client, "good-token");

        // Data still comes through...
        var mealPlans = json.RootElement.GetProperty("data").GetProperty("dashboardMealPlans");
        Assert.Equal(1, mealPlans.GetArrayLength());
        Assert.Equal(mealPlanId.ToString(), mealPlans[0].GetProperty("id").GetString());

        // ...alongside a field-level error, rather than failing silently.
        Assert.True(json.RootElement.TryGetProperty("errors", out var errors));
        Assert.Contains("meal details omitted", errors[0].GetProperty("message").GetString());
    }

    [Fact]
    public async Task Query_UpstreamRequiredCallFails_ReturnsGraphQLError()
    {
        var dashboardServiceMock = new Mock<IDashboardService>();
        dashboardServiceMock
            .Setup(s => s.GetDashboardMealPlansAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), "good-token", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UpstreamServiceException("PlanService", 500));

        var client = CreateClientWithMockedDashboard(dashboardServiceMock);
        var json = await PostGraphQL(client, "good-token");

        Assert.True(json.RootElement.TryGetProperty("errors", out var errors));
        Assert.Contains("PlanService", errors[0].GetProperty("message").GetString());
    }
}
