using GatewayBff.Services;
using GatewayBff.Types;
using HotChocolate;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;

namespace GatewayBff;

public class Query
{
    [GraphQLName("dashboardMealPlans")]
    public async Task<IEnumerable<DashboardMealPlan>> GetDashboardMealPlans(
        DateTime startDate,
        DateTime endDate,
        [Service] IDashboardService dashboardService,
        [Service] IHttpContextAccessor httpContextAccessor,
        IResolverContext resolverContext,
        CancellationToken cancellationToken)
    {
        var token = ExtractBearerToken(httpContextAccessor);
        if (token is null)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage("Missing or invalid Authorization header.")
                .SetCode("UNAUTHENTICATED")
                .Build());
        }

        DashboardResult result;
        try
        {
            result = await dashboardService.GetDashboardMealPlansAsync(startDate, endDate, token, cancellationToken);
        }
        catch (UpstreamServiceException ex)
        {
            throw new GraphQLException(ErrorBuilder.New()
                .SetMessage(ex.Message)
                .SetCode("UPSTREAM_ERROR")
                .Build());
        }

        if (result.PartialFailureMessage is not null)
        {
            resolverContext.ReportError(ErrorBuilder.New()
                .SetMessage(result.PartialFailureMessage)
                .SetCode("UPSTREAM_PARTIAL_FAILURE")
                .Build());
        }

        return result.MealPlans;
    }

    public static string? ExtractBearerToken(IHttpContextAccessor accessor)
    {
        var header = accessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;

        var token = header["Bearer ".Length..].Trim();
        return token.Length == 0 ? null : token;
    }
}
