using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    /// <summary>
    /// Håndterer innlogging og utlogging for alle brukere.
    /// Brukere logges inn samme sted og sendes videre basert på rolle.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager   = userManager;
        }

        // ---------- Login (GET/POST) ----------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginVm());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginVm vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            // Finn bruker på e-post
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                // Ikke avslør om brukeren finnes
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(vm);
            }

            // Forsøk å logge inn
            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(vm);
            }

            // Hvis vi har en lokal returnUrl (f.eks. etter [Authorize])
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Rollebasert redirect
            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin))
            {
                // Admin
                return RedirectToAction("Index", "Admin");
            }

            if (await _userManager.IsInRoleAsync(user, AppRoles.Registrar))
            {
                // Registerfører
                return RedirectToAction("Overview", "Obstacle");
            }

            if (await _userManager.IsInRoleAsync(user, AppRoles.Pilot))
            {
                // Pilot
                return RedirectToAction("Index", "Home");
            }

            // Bruker uten gyldig rolle → logg ut og vis feil
            await _signInManager.SignOutAsync();
            ModelState.AddModelError(string.Empty, "Your account does not have a valid role.");
            return View(vm);
        }

        // ---------- Logout ----------

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        // ---------- AccessDenied ----------

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}
