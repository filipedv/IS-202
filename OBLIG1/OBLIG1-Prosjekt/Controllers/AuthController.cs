using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;
using System.ComponentModel.DataAnnotations;

namespace OBLIG1.Controllers
{
    // ViewModel for login
    public class LoginVm
    {
        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
    }

    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----- Forgot Password -----
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Please enter your email address.");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with that email.");
                return View();
            }

            // TODO: send reset email
            ViewBag.Message = "Password reset link sent to your email.";
            return View();
        }

        // ----- Pilot Login -----
        [HttpGet]
        public IActionResult Index() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PilotLogin(LoginVm vm)
        {
            if (!ModelState.IsValid)
                return View("Index", vm);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == vm.Email && u.Password == vm.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View("Index", vm);
            }

            if (user.Role != "Pilot")
            {
                ModelState.AddModelError("", "You are not a pilot");
                return View("Index", vm);
            }

            // --- Security claims from Fikse ---
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        // ----- Registerer Login -----
        [HttpGet]
        public IActionResult Registerforer() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registerforer(LoginVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == vm.Email && u.Password == vm.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(vm);
            }

            if (user.Role != "Registerer")
            {
                ModelState.AddModelError("", "You are not a registerer");
                return View(vm);
            }

            // --- Security claims ---
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Overview", "Obstacle");
        }

        // ----- Admin Login -----
        [HttpGet]
        public IActionResult Admin() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin(LoginVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == vm.Email && u.Password == vm.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(vm);
            }

            if (user.Role != "Admin")
            {
                ModelState.AddModelError("", "You are not an admin");
                return View(vm);
            }

            // --- Security claims ---
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Users", "Admin");
        }

        // ----- Log Out -----
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}
