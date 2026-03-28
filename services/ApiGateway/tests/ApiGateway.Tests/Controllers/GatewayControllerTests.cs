using ApiGateway.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ApiGateway.Tests.Controllers;

public class GatewayControllerTests
{
    private readonly GatewayController _controller;

    public GatewayControllerTests()
    {
        _controller = new GatewayController();
    }

    // --- GetStatus ---

    [Fact]
    public void GetStatus_ReturnsOk()
    {
        var result = _controller.GetStatus();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetStatus_ReturnsStatusMessage()
    {
        var result = _controller.GetStatus();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);

        var status = ok.Value!.GetType().GetProperty("status")?.GetValue(ok.Value) as string;
        Assert.Equal("ApiGateway is running", status);
    }
}
