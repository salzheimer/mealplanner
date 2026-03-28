using Microsoft.AspNetCore.Mvc;
using PlanService.Controllers;
using Xunit;

namespace PlanService.Tests.Controllers;

public class PlansControllerTests
{
    private readonly PlansController _controller;

    public PlansControllerTests()
    {
        _controller = new PlansController();
    }

    // --- GetAll ---

    [Fact]
    public void GetAll_ReturnsOk()
    {
        var result = _controller.GetAll();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void GetAll_ReturnsEmptyCollection()
    {
        var result = _controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var plans = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
        Assert.Empty(plans);
    }

    // --- GetById ---

    [Fact]
    public void GetById_NonExistentId_ReturnsNotFound()
    {
        var result = _controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetById_ZeroId_ReturnsNotFound()
    {
        var result = _controller.GetById(0);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}
