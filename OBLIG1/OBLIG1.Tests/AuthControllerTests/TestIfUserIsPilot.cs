using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OBLIG1.Controllers;
using OBLIG1.Models;
using OBLIG1.ViewModels;

namespace OBLIG1.Tests;

public class AuthController_PilotLoginTests
{
    [Fact]
    public async Task Index_Post_ShouldRedirectToHome_WhenUserIsPilot()
    {
        // Arrange
        var userManagerMock = AuthTestHelper.CreateUserManagerMock();
        var signInManagerMock = AuthTestHelper.CreateSignInManagerMock(userManagerMock.Object);

        var controller = new AuthController(signInManagerMock.Object, userManagerMock.Object);

        var user = new ApplicationUser
        {
            Email = "pilot@test.com",
            UserName = "pilot@test.com"
        };

        var vm = new LoginVm
        {
            Email = "pilot@test.com",
            Password = "Pilot1!"
        };

        // 1) Brukeren finnes
        userManagerMock.Setup(x => x.FindByEmailAsync(vm.Email))
            .ReturnsAsync(user);

        // 2) Passordet er riktig
        signInManagerMock.Setup(x =>
                x.PasswordSignInAsync(vm.Email, vm.Password, false, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // 3) Rollen er Pilot (ikke Admin, ikke Registrar)
        userManagerMock.Setup(x => x.IsInRoleAsync(user, AppRoles.Admin))
            .ReturnsAsync(false);

        userManagerMock.Setup(x => x.IsInRoleAsync(user, AppRoles.Registrar))
            .ReturnsAsync(false);

        userManagerMock.Setup(x => x.IsInRoleAsync(user, AppRoles.Pilot))
            .ReturnsAsync(true);

        // Act
        var result = await controller.Index(vm, null);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }
}