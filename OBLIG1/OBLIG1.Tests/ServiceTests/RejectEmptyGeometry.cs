using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using OBLIG1.Controllers;
using OBLIG1.Models;
using OBLIG1.Services;
using Xunit;

namespace OBLIG1.Tests;
// Hjelpemetode som lager en ObstacleController for tester,
// med en fake innlogget bruker, HttpContext og TempData satt opp.
public class RejectEmptyGeometry
{
    private ObstacleController CreateController(Mock<IObstacleService> mockService)
    {
        var controller = new ObstacleController(mockService.Object);

        // Sett opp fake bruker
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "user-123") };
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };
        controller.TempData = new TempDataDictionary(
            controller.ControllerContext.HttpContext,
            Mock.Of<ITempDataProvider>());

        return controller;
    }

    // Sjekker at man ikke kan registrere et hinder uten å markere et punkt på kartet
    [Fact]
    public async Task DataForm_ShouldRejectEmptyGeometry()
    {
        // Arrange
        var mockService = new Mock<IObstacleService>();
        var controller = CreateController(mockService);

        var vm = new ObstacleData { GeometryGeoJson = "" };

        // Act
        var result = await controller.DataForm(vm);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        
        // Verifiser at CreateAsync aldri ble kalt (hinderet ble ikke lagret)
        mockService.Verify(
            s => s.CreateAsync(It.IsAny<ObstacleData>(), It.IsAny<string>()), 
            Times.Never);
    }
    
// Tester at DataForm avviser en modell uten geometry:
// ModelState skal være ugyldig og CreateAsync skal aldri kalles.
    [Fact]
    public async Task DataForm_ShouldRejectNullGeometry()
    {
        // Arrange
        var mockService = new Mock<IObstacleService>();
        var controller = CreateController(mockService);

        var vm = new ObstacleData { GeometryGeoJson = null };

        // Act
        var result = await controller.DataForm(vm);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        mockService.Verify(
            s => s.CreateAsync(It.IsAny<ObstacleData>(), It.IsAny<string>()), 
            Times.Never);
    }
// Tester at DataForm avviser geometry som bare inneholder whitespace
// Modellen skal være ugyldig og vi får tilbake et ViewResult.
    [Fact]
    public async Task DataForm_ShouldRejectWhitespaceOnlyGeometry()
    {
        // Arrange
        var mockService = new Mock<IObstacleService>();
        var controller = CreateController(mockService);

        var vm = new ObstacleData { GeometryGeoJson = "   " };

        // Act
        var result = await controller.DataForm(vm);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }
}
