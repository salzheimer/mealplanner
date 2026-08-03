using System.Net;
using System.Text;
using GatewayBff.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;

namespace GatewayBff.Tests;

public class DashboardServiceTests
{
    private const string PlanBaseUrl = "http://plan-service.test";
    private const string MealBaseUrl = "http://meal-service.test";
    private const string Token = "test-token";

    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Services:PlanService"] = PlanBaseUrl,
                ["Services:MealRecipeService"] = MealBaseUrl,
            })
            .Build();

    private static HttpClient FakeClient(HttpStatusCode status, string? json = null)
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(status)
            {
                Content = json is null ? null : new StringContent(json, Encoding.UTF8, "application/json")
            });
        return new HttpClient(handler.Object) { BaseAddress = new Uri("http://fake-service") };
    }

    [Fact]
    public async Task GetDashboardMealPlansAsync_BothCallsSucceed_ReturnsJoinedResult()
    {
        var mealPlanId = Guid.NewGuid();
        var mealId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var mealPlansJson = $$"""
        [
          {
            "id": "{{mealPlanId}}",
            "mealId": "{{mealId}}",
            "planId": "{{planId}}",
            "serveDate": "2026-08-10T00:00:00Z",
            "endDate": null,
            "addedByUserId": "{{ownerId}}",
            "createdAt": "2026-08-01T00:00:00Z",
            "updatedAt": "2026-08-01T00:00:00Z"
          }
        ]
        """;

        var mealsJson = $$"""
        [
          {
            "id": "{{mealId}}",
            "name": "Monday Dinner",
            "description": null,
            "notes": null,
            "mealTypeId": 1,
            "isMultiDayMeal": false,
            "ownerUserId": "{{ownerId}}"
          }
        ]
        """;

        var factory = new Mock<IServiceClientFactory>();
        factory.Setup(f => f.CreateClient(PlanBaseUrl, Token)).Returns(FakeClient(HttpStatusCode.OK, mealPlansJson));
        factory.Setup(f => f.CreateClient(MealBaseUrl, Token)).Returns(FakeClient(HttpStatusCode.OK, mealsJson));

        var service = new DashboardService(BuildConfiguration(), factory.Object);

        var result = await service.GetDashboardMealPlansAsync(
            DateTime.UtcNow, DateTime.UtcNow.AddDays(7), Token, CancellationToken.None);

        Assert.Null(result.PartialFailureMessage);
        var mealPlan = Assert.Single(result.MealPlans);
        Assert.Equal(mealPlanId, mealPlan.Id);
        Assert.NotNull(mealPlan.Meal);
        Assert.Equal("Monday Dinner", mealPlan.Meal!.Name);
    }

    [Fact]
    public async Task GetDashboardMealPlansAsync_PlanServiceFails_ThrowsUpstreamServiceException()
    {
        var factory = new Mock<IServiceClientFactory>();
        factory.Setup(f => f.CreateClient(PlanBaseUrl, Token)).Returns(FakeClient(HttpStatusCode.InternalServerError));

        var service = new DashboardService(BuildConfiguration(), factory.Object);

        var ex = await Assert.ThrowsAsync<UpstreamServiceException>(() =>
            service.GetDashboardMealPlansAsync(DateTime.UtcNow, DateTime.UtcNow.AddDays(7), Token, CancellationToken.None));

        Assert.Equal("PlanService", ex.ServiceName);
        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public async Task GetDashboardMealPlansAsync_MealServiceFails_ReturnsMealPlansWithNullMealsAndPartialFailureMessage()
    {
        var mealPlanId = Guid.NewGuid();
        var mealId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var mealPlansJson = $$"""
        [
          {
            "id": "{{mealPlanId}}",
            "mealId": "{{mealId}}",
            "planId": "{{planId}}",
            "serveDate": null,
            "endDate": null,
            "addedByUserId": "{{ownerId}}",
            "createdAt": "2026-08-01T00:00:00Z",
            "updatedAt": "2026-08-01T00:00:00Z"
          }
        ]
        """;

        var factory = new Mock<IServiceClientFactory>();
        factory.Setup(f => f.CreateClient(PlanBaseUrl, Token)).Returns(FakeClient(HttpStatusCode.OK, mealPlansJson));
        factory.Setup(f => f.CreateClient(MealBaseUrl, Token)).Returns(FakeClient(HttpStatusCode.ServiceUnavailable));

        var service = new DashboardService(BuildConfiguration(), factory.Object);

        var result = await service.GetDashboardMealPlansAsync(
            DateTime.UtcNow, DateTime.UtcNow.AddDays(7), Token, CancellationToken.None);

        Assert.NotNull(result.PartialFailureMessage);
        var mealPlan = Assert.Single(result.MealPlans);
        Assert.Equal(mealPlanId, mealPlan.Id);
        Assert.Null(mealPlan.Meal);
    }

    [Fact]
    public async Task GetDashboardMealPlansAsync_NoMealPlans_ReturnsEmptyList()
    {
        var factory = new Mock<IServiceClientFactory>();
        factory.Setup(f => f.CreateClient(PlanBaseUrl, Token)).Returns(FakeClient(HttpStatusCode.OK, "[]"));
        factory.Setup(f => f.CreateClient(MealBaseUrl, Token)).Returns(FakeClient(HttpStatusCode.OK, "[]"));

        var service = new DashboardService(BuildConfiguration(), factory.Object);

        var result = await service.GetDashboardMealPlansAsync(
            DateTime.UtcNow, DateTime.UtcNow.AddDays(7), Token, CancellationToken.None);

        Assert.Empty(result.MealPlans);
        Assert.Null(result.PartialFailureMessage);
    }
}
