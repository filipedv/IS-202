using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OBLIG1.Controllers
{
    // Enkel viewmodel for innlogging
    public class LoginVm
    {
        //[Required, EmailAddress] -- Fjerne "//" og ? i string når vi har brukere, dette er bare test av routingen
        public string? Email { get; set; } //= "";

        //[Required, DataType(DataType.Password)] -- Fjerne "//" og ? i string når vi har brukere, dette er bare test av routingen
        public string? Password { get; set; } //= "";
    }

    public class AuthController : Controller
    {
        // Førstesiden – viser pilot-innlogging + knapper til de andre
        [HttpGet]
        public IActionResult Index() => View(new LoginVm());

        // Pilot-login (POST fra forsiden)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PilotLogin(LoginVm vm)
        {
            if (!ModelState.IsValid) return View("Index", vm);

            
            // if (OK) -> redirect til pilot-dashboard
            return RedirectToAction("Index", "Home"); // midlertidig
        }

        // Registerfører – egen innloggingsside
        [HttpGet]
        public IActionResult Registerforer() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registerforer(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
           
            return RedirectToAction("Overview", "Obstacle"); // f.eks. oversiktssiden
        }

        // Admin – egen innloggingsside
        [HttpGet]
        public IActionResult Admin() => View(new LoginVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Admin(LoginVm vm)
        {
            if (!ModelState.IsValid) return View(vm);
           
            return RedirectToAction("Index", "Home");
        }
    }
}