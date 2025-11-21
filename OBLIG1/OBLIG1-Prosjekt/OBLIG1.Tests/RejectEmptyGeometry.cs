using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Controllers;
using OBLIG1.Data;
using OBLIG1.Models;

namespace OBLIG1.Tests;

public class RejectEmptyGeometry
{
    [Fact]
    public async Task DataForm_ShouldNotSaveObstacle_WhenGeometryIsEmpty()
    {
        // ARRANGE
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("RejectEmptyGeo")
            .Options;

        using var db = new ApplicationDbContext(options);
        var controller = new ObstacleController(db);

        var vm = new ObstacleData { GeometryGeoJson = "" };

        // ACT
        var result = await controller.DataForm(vm);

        // ASSERT
        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.Equal(0, db.Obstacles.Count());
    }
}