using Xunit;
using Microsoft.AspNetCore.Mvc;
using OBLIG1.Controllers;
using OBLIG1.Models;
using OBLIG1.ViewModels;

namespace OBLIG1.Tests;

public class AuthController_InvalidModelTests
{
    [Fact]
    public async Task Index_Post_ShouldReturnView_WhenModelStateIsInvalid()
    {
        // Arrange
        var userManagerMock = AuthTestHelper.CreateUserManagerMock();
        var signInManagerMock = AuthTestHelper.CreateSignInManagerMock(userManagerMock.Object);

        var controller = new AuthController(signInManagerMock.Object, userManagerMock.Object);
        controller.ModelState.AddModelError("Email", "Required"); // simulerer ugyldig modell

        var vm = new LoginVm(); // tom/ugyldig innlogging-modell

        // Act
        var result = await controller.Index(vm, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);   // skal returnere View, ikke redirect
        Assert.IsType<LoginVm>(viewResult.Model);             // modellen tilbake til viewet er LoginVm
    }
}