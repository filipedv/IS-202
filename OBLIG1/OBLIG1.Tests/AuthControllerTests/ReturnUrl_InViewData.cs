using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using OBLIG1.Controllers;
using OBLIG1.Models;
using OBLIG1.ViewModels;   // hvis LoginVm ligger her

namespace OBLIG1.Tests;

public class AuthControllerTest_ReturnUrl
{
    [Fact]
    public void Index_Get_ShouldSetReturnUrlInViewData()
    {
        
        var userManagerMock = AuthTestHelper.CreateUserManagerMock();
        var signInManagerMock = AuthTestHelper.CreateSignInManagerMock(userManagerMock.Object);

        var controller = new AuthController(signInManagerMock.Object, userManagerMock.Object);
        var returnUrl = "/Obstacle/Overview";

        // Metoden skal returne ViewResult
        var result = controller.Index(returnUrl);

        
        var viewResult = Assert.IsType<ViewResult>(result);
        // Controlleren skal lagre returnUrl i ViewData
        Assert.Equal(returnUrl, viewResult.ViewData["ReturnUrl"]);
    }
}