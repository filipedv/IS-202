using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;
using System.Linq;
using System.ComponentModel.DataAnnotations;



namespace OBLIG1.Controllers
{
    // Simple ViewModel for login
    public class LoginVm
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        //Forgot Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Please enter your email address.");
                return View();
            }

            // Example: find user and send reset link
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with that email.");
                return View();
            }

            // TODO: generate and email reset token (for now, simulate)
            ViewBag.Message = "Password reset link sent to your email.";
            return View();
        }


        // Pilot login page (default)
        [HttpGet]
        public IActionResult Index() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PilotLogin(LoginVm vm)
        {
            if (!ModelState.IsValid) return View("Index", vm);

            var user = _context.Users
                .FirstOrDefault(u => u.Email == vm.Email && u.Password == vm.Password);

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

            // Pilot dashboard
            return RedirectToAction("Index", "Home");
        }

        // Registerer login page
        [HttpGet]
        public IActionResult Registerforer() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registerforer(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = _context.Users
                .FirstOrDefault(u => u.Email == vm.Email && u.Password == vm.Password);

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

            // Registerer dashboard
            return RedirectToAction("Overview", "Obstacle");
        }

        // Admin login page
        [HttpGet]
        public IActionResult Admin() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Admin(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = _context.Users
                .FirstOrDefault(u => u.Email == vm.Email && u.Password == vm.Password);

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

            // Admin dashboard
            return RedirectToAction("Users", "Admin");
        }
    }
}
