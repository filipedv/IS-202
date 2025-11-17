using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OBLIG1.Controllers
{
    // Enkel viewmodel for innlogging
    public class LoginVm
    {
        // Når dere får ordentlig validering/brukere kan dere skru på disse:
        //[Required, EmailAddress]
        public string? Email { get; set; }

        //[Required, DataType(DataType.Password)]
        public string? Password { get; set; }
    }

    public class AuthController : Controller
    {
        // Førstesiden – viser pilot-innlogging + knapper til de andre
        [HttpGet]
        public IActionResult Index() => View(new LoginVm());

        // ---------- Pilot-login ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PilotLogin(LoginVm vm)
        {
            if (!ModelState.IsValid)
                return View("Index", vm);

            // Her kunne du normalt sjekket brukernavn/passord.
            // Nå faker vi en innlogging med rolle "Pilot":

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, vm.Email ?? "pilot@example.com"),
                new Claim(ClaimTypes.Name,          vm.Email ?? "Pilot"),
                new Claim(ClaimTypes.Role,          "Pilot")   // <-- VIKTIG: matcher [Authorize(Roles="Pilot,...")]
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }

        // ---------- Registerfører-login ----------
        [HttpGet]
        public IActionResult Registerforer() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registerforer(LoginVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, vm.Email ?? "registrar@example.com"),
                new Claim(ClaimTypes.Name,          vm.Email ?? "Registrar"),
                new Claim(ClaimTypes.Role,          "Registerforer") // matcher [Authorize(Roles="Pilot,Registerforer")]
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Overview", "Obstacle");
        }

        // ---------- Admin-login (valgfritt role-navn) ----------
        [HttpGet]
        public IActionResult Admin() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin(LoginVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, vm.Email ?? "admin@example.com"),
                new Claim(ClaimTypes.Name,          vm.Email ?? "Admin"),
                new Claim(ClaimTypes.Role,          "Admin")
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }

        // (valgfritt) Log out
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}
