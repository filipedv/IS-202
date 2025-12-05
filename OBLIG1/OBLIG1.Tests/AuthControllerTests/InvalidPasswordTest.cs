/*using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OBLIG1.Controllers;
using OBLIG1.Models;
using OBLIG1.ViewModels;

namespace OBLIG1.Tests;

public class AuthController_InvalidPasswordTests
{
    [Fact]
    public async Task Index_Post_ShouldReturnViewWithError_WhenPasswordIsWrong()
    {
        // Arrange
        var userManagerMock = AuthTestHelper.CreateUserManagerMock();
        var signInManagerMock = AuthTestHelper.CreateSignInManagerMock(userManagerMock.Object);

        var controller = new AuthController(signInManagerMock.Object, userManagerMock.Object);

        var user = new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test@test.com"
        };

        var vm = new LoginVm
        {
            Email = "test@test.com",
            Password = "WrongPassword!"
        };

        userManagerMock
            .Setup(x => x.FindByEmailAsync(vm.Email))
            .ReturnsAsync(user);

        signInManagerMock
            .Setup(x => x.PasswordSignInAsync(
                It.IsAny<ApplicationUser>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);


        // Act
        var result = await controller.Index(vm, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }
}
*/
