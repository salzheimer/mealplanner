using System.Net.Http.Json;
using System.Text.Json;
using GatewayBff.Types;

namespace GatewayBff.Services;

public class UpstreamServiceException(string serviceName, int statusCode)
    : Exception($"{serviceName} returned HTTP {statusCode}.")
{
    public string ServiceName { get; } = serviceName;
    public int StatusCode { get; } = statusCode;
}

public record DashboardResult(IReadOnlyList<DashboardMealPlan> MealPlans, string? PartialFailureMessage);

public interface IDashboardService
{
    Task<DashboardResult> GetDashboardMealPlansAsync(
        DateTime startDate, DateTime endDate, string bearerToken, CancellationToken cancellationToken);
}

/// <summary>
/// Composes PlanService's date-range meal plans with MealRecipeService's meal list —
/// the same join DashboardPage.tsx currently does client-side across two REST calls.
/// The meal-plans call is required (a failure fails the whole field); the meals call
/// is treated as best-effort so a MealRecipeService outage degrades to meal plans
/// without embedded meal details rather than failing the entire dashboard.
/// </summary>
public class DashboardService : IDashboardService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly IServiceClientFactory _clientFactory;
    private readonly string _planServiceBaseUrl;
    private readonly string _mealRecipeServiceBaseUrl;

    public DashboardService(IConfiguration configuration, IServiceClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
        _planServiceBaseUrl = configuration["Services:PlanService"]
            ?? throw new InvalidOperationException("Services:PlanService is not configured.");
        _mealRecipeServiceBaseUrl = configuration["Services:MealRecipeService"]
            ?? throw new InvalidOperationException("Services:MealRecipeService is not configured.");
    }

    public async Task<DashboardResult> GetDashboardMealPlansAsync(
        DateTime startDate, DateTime endDate, string bearerToken, CancellationToken cancellationToken)
    {
        using var planClient = _clientFactory.CreateClient(_planServiceBaseUrl, bearerToken);
        var mealPlansResponse = await planClient.GetAsync(
            $"/api/mealplan/date-range?startDate={Uri.EscapeDataString(startDate.ToString("O"))}&endDate={Uri.EscapeDataString(endDate.ToString("O"))}",
            cancellationToken);

        if (!mealPlansResponse.IsSuccessStatusCode)
            throw new UpstreamServiceException("PlanService", (int)mealPlansResponse.StatusCode);

        var mealPlans = await mealPlansResponse.Content.ReadFromJsonAsync<List<PlanMealPlanDto>>(JsonOptions, cancellationToken)
            ?? [];

        string? partialFailureMessage = null;
        var mealsById = new Dictionary<Guid, MealSummary>();

        using var mealClient = _clientFactory.CreateClient(_mealRecipeServiceBaseUrl, bearerToken);
        var mealsResponse = await mealClient.GetAsync("/api/meal", cancellationToken);

        if (mealsResponse.IsSuccessStatusCode)
        {
            var meals = await mealsResponse.Content.ReadFromJsonAsync<List<MealSummaryDto>>(JsonOptions, cancellationToken)
                ?? [];
            foreach (var meal in meals)
            {
                mealsById[meal.Id] = new MealSummary(
                    meal.Id, meal.Name, meal.Description, meal.MealTypeId, meal.IsMultiDayMeal, meal.OwnerUserId);
            }
        }
        else
        {
            partialFailureMessage = $"MealRecipeService returned HTTP {(int)mealsResponse.StatusCode}; meal details omitted.";
        }

        var mealPlanResults = mealPlans
            .Select(mp => new DashboardMealPlan(
                mp.Id, mp.MealId, mp.PlanId, mp.ServeDate, mp.EndDate,
                mealsById.GetValueOrDefault(mp.MealId)))
            .ToList();

        return new DashboardResult(mealPlanResults, partialFailureMessage);
    }
}
