/*
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

public class RejectEmptyGeometry
{
    //sjekker at man ikke kan registrere et hinder uten å markere et punkt på kartet
    [Fact]
    public async Task DataForm_ShouldRejectEmptyGeometry()
    {
        // Arrange
        var mockService = new Mock<IObstacleService>();
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

        var vm = new ObstacleData { GeometryGeoJson = "" };

        // Act
        var result = await controller.DataForm(vm);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }
}
*/