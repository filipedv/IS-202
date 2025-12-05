using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OBLIG1.Controllers;
using OBLIG1.Models;

namespace OBLIG1.Tests;

public class InvalidPasswordTest
{
    [Fact]
    public async Task Index_Post_ShouldReturnViewWithError_WhenPasswordIsWrong()
    {
        //vi setter opp falske (mock) versjoner av UserManager og SignInManager
        var userManagerMock = AuthTestHelper.CreateUserManagerMock();
        var signInManagerMock = AuthTestHelper.CreateSignInManagerMock(userManagerMock.Object);

        // Oppretter AuthController med mockene
        var controller = new AuthController(signInManagerMock.Object, userManagerMock.Object);

        var user = new ApplicationUser
        {
            Email = "test@test.com",
            UserName = "test@test.com"
        };
        
        // ViewModel for å simulere en innloggingsforespørsel med feil passord
        var vm = new LoginVm
        {
            Email = "test@test.com",
            Password = "WrongPassword!"
        };

        // Når controlleren prøver å hente brukeren  returner vår testbruker
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

        // controlleren returnerer en ViewResult  (ikke redirect)
        var result = await controller.Index(vm, null);

        
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(string.Empty));
        Assert.Contains("Invalid login attempt", 
            controller.ModelState[string.Empty]!.Errors.First().ErrorMessage);
    }
}