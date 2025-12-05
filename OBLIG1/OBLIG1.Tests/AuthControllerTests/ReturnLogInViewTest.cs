using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using OBLIG1.Controllers;
using OBLIG1.Models;
using OBLIG1.ViewModels;   // hvis LoginVm ligger her

namespace OBLIG1.Tests;

public class AuthControllerTest_LogInView
{
    [Fact]
    public void Index_Get_ShouldReturnViewWithLoginVm()
    {
        // Arrange
        var userManagerMock = AuthTestHelper.CreateUserManagerMock();
        var signInManagerMock = AuthTestHelper.CreateSignInManagerMock(userManagerMock.Object);

        var controller = new AuthController(signInManagerMock.Object, userManagerMock.Object);

        // Act
        var result = controller.Index(null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<LoginVm>(viewResult.Model);
    }
}