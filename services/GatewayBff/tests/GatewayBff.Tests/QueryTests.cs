using Microsoft.AspNetCore.Http;
using Xunit;

namespace GatewayBff.Tests;

public class QueryExtractBearerTokenTests
{
    private static IHttpContextAccessor AccessorWithHeader(string? authorizationHeaderValue)
    {
        var context = new DefaultHttpContext();
        if (authorizationHeaderValue is not null)
            context.Request.Headers.Authorization = authorizationHeaderValue;

        return new HttpContextAccessor { HttpContext = context };
    }

    [Fact]
    public void ExtractBearerToken_ValidBearerHeader_ReturnsToken()
    {
        var accessor = AccessorWithHeader("Bearer abc123");

        var token = Query.ExtractBearerToken(accessor);

        Assert.Equal("abc123", token);
    }

    [Fact]
    public void ExtractBearerToken_NoHeader_ReturnsNull()
    {
        var accessor = AccessorWithHeader(null);

        Assert.Null(Query.ExtractBearerToken(accessor));
    }

    [Fact]
    public void ExtractBearerToken_NonBearerScheme_ReturnsNull()
    {
        var accessor = AccessorWithHeader("Basic dXNlcjpwYXNz");

        Assert.Null(Query.ExtractBearerToken(accessor));
    }

    [Fact]
    public void ExtractBearerToken_BearerWithNoToken_ReturnsNull()
    {
        var accessor = AccessorWithHeader("Bearer ");

        Assert.Null(Query.ExtractBearerToken(accessor));
    }

    [Fact]
    public void ExtractBearerToken_NoHttpContext_ReturnsNull()
    {
        var accessor = new HttpContextAccessor { HttpContext = null };

        Assert.Null(Query.ExtractBearerToken(accessor));
    }
}
