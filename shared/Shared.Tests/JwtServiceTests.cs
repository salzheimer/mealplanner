using Shared.Services;
using Xunit;

namespace Shared.Tests;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _jwtService = new JwtService("test-issuer", "test-audience", "test-secret-key-must-be-32-chars!!");
    }

    [Fact]
    public void GenerateToken_ValidInputs_ReturnsNonEmptyString()
    {
        var token = _jwtService.GenerateToken(1, "testuser", TimeSpan.FromMinutes(60));

        Assert.NotEmpty(token);
    }

    [Fact]
    public void ValidateToken_ValidToken_ReturnsPrincipal()
    {
        var token = _jwtService.GenerateToken(42, "alice", TimeSpan.FromMinutes(60));

        var principal = _jwtService.ValidateToken(token);

        Assert.NotNull(principal);
    }

    [Fact]
    public void ValidateToken_ValidToken_ContainsExpectedClaims()
    {
        var token = _jwtService.GenerateToken(42, "alice", TimeSpan.FromMinutes(60));

        var principal = _jwtService.ValidateToken(token);

        Assert.NotNull(principal);
        Assert.Equal("42", principal.FindFirst("sub")?.Value);
        Assert.Equal("alice", principal.FindFirst("unique_name")?.Value);
    }

    [Fact]
    public void ValidateToken_InvalidToken_ReturnsNull()
    {
        var principal = _jwtService.ValidateToken("not.a.valid.token");

        Assert.Null(principal);
    }

    [Fact]
    public void ValidateToken_ExpiredToken_ReturnsNull()
    {
        var token = _jwtService.GenerateToken(1, "testuser", TimeSpan.FromSeconds(-1));

        var principal = _jwtService.ValidateToken(token);

        Assert.Null(principal);
    }

    [Fact]
    public void ValidateToken_TokenFromDifferentSecret_ReturnsNull()
    {
        var otherService = new JwtService("test-issuer", "test-audience", "different-secret-key-32-chars!!!!");
        var token = otherService.GenerateToken(1, "testuser", TimeSpan.FromMinutes(60));

        var principal = _jwtService.ValidateToken(token);

        Assert.Null(principal);
    }
}
