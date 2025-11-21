using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    public class LoginVm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }

    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Førstesiden – pilot-login som default
        [HttpGet]
        public IActionResult Index() => View(new LoginVm());

        // -------- Pilot-login --------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PilotLogin(LoginVm vm)
        {
            if (!ModelState.IsValid) return View("Index", vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View("Index", vm);
            }

            // Sjekk at brukeren faktisk har rollen Pilot
            if (!await _userManager.IsInRoleAsync(user, "Pilot"))
            {
                ModelState.AddModelError("", "You are not a Pilot user.");
                return View("Index", vm);
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View("Index", vm);
            }

            return RedirectToAction("Index", "Home");
        }

        // -------- Registerfører-login --------
        [HttpGet]
        public IActionResult Registerforer() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registerforer(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            if (!await _userManager.IsInRoleAsync(user, "Registerforer"))
            {
                ModelState.AddModelError("", "You are not a Registrar user.");
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            return RedirectToAction("Overview", "Obstacle");
        }

        // -------- Admin-login --------
        [HttpGet]
        public IActionResult Admin() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "You are not an Admin user.");
                return View(vm);
            }

            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            return RedirectToAction("Index", "Admin");
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        // Valgfritt AccessDenied-view
        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
