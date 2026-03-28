using MealService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Xunit;

namespace MealService.Tests.Controllers;

public class RecipesControllerTests
{
    private readonly RecipesController _controller;

    public RecipesControllerTests()
    {
        _controller = new RecipesController();
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
        var recipes = Assert.IsAssignableFrom<IEnumerable<RecipeDto>>(ok.Value);
        Assert.Empty(recipes);
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
