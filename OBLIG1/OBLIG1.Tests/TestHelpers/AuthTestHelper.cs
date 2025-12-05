using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OBLIG1.Models;

namespace OBLIG1.Tests;

public static class AuthTestHelper
{
    /// Lager en mock av UserManager<ApplicationUser> slik at vi kan teste AuthController
    /// uten at Identity prøver å snakke med en ekte database.
    public static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        // Identity trenger et IUserStore, så vi mocker dette også
        var store = new Mock<IUserStore<ApplicationUser>>();

        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());

        var userValidators = new List<IUserValidator<ApplicationUser>>();
        var pwdValidators = new List<IPasswordValidator<ApplicationUser>>();

        // Returnerer en ferdig konfigurert UserManager-mock
        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            options.Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            userValidators,
            pwdValidators,
            new Mock<ILookupNormalizer>().Object,
            new IdentityErrorDescriber(),
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object
        );
    }

    /// Lager en mock av SignInManager<ApplicationUser> slik at vi kan simulere
    /// vellykket / mislykket innlogging i tester.
    public static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(
        UserManager<ApplicationUser> userManager)
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        
        // IdentityOptions for innlogging
        var options = new Mock<IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        
        // Logger og andre avhengigheter mockes siden de ikke brukes i testene
        var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

        // Returnerer mocket SignInManage
        return new Mock<SignInManager<ApplicationUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            options.Object,
            logger.Object,
            schemes.Object,
            confirmation.Object
        );
    }
}
