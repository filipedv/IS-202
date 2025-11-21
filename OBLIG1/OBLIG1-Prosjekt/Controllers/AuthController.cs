using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    // ViewModel for innlogging – brukes i flere login-views
    public class LoginVm
    {
        // Påkrevd e-postadresse, med validering på e-postformat
        [Required, EmailAddress]
        public string Email { get; set; } = "";
        // Påkrevd passordfelt, rendres som passord i UI
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }

    // Kontroller som håndterer autentisering (innlogging/utlogging) for ulike roller
    public class AuthController : Controller
    {
        // Håndterer selve innloggings-/utloggingsprosessen (cookies, lockout osv.)
        private readonly SignInManager<ApplicationUser> _signInManager;
        // Håndterer brukere (henting av bruker, sjekke roller osv.)
        private readonly UserManager<ApplicationUser> _userManager;

        // Konstruktør med dependency injection av SignInManager og UserManager
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
        
        // Behandler innsending av pilot-innlogging fra førstesiden
        [HttpPost]
        [ValidateAntiForgeryToken] // Beskytter mot CSRF-angrep
        public async Task<IActionResult> PilotLogin(LoginVm vm)
        {
            // Sjekk server-side validering av innloggingsskjema
            if (!ModelState.IsValid) return View("Index", vm);

            // Forsøk å finne bruker basert på e-post
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                // Ikke avslør om brukeren finnes eller ikke, generisk feilmelding
                ModelState.AddModelError("", "Invalid login attempt.");
                return View("Index", vm);
            }

            // Sjekk at brukeren faktisk har rollen Pilot
            if (!await _userManager.IsInRoleAsync(user, "Pilot"))
            {
                ModelState.AddModelError("", "You are not a Pilot user.");
                return View("Index", vm);
            }
            
            // Forsøk å logge inn med e-postbruker og passord, uten å huske innlogging og uten lockout
            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);
            if (!result.Succeeded)
            {
                // Feil passord eller annen innloggingsfeil
                ModelState.AddModelError("", "Invalid login attempt.");
                return View("Index", vm);
            }

            // Ved vellykket innlogging sendes Pilot til Home-siden
            return RedirectToAction("Index", "Home");
        }

        // -------- Registerfører-login --------
        
        // Viser eget innloggingsskjema for Registerfører
        [HttpGet]
        public IActionResult Registerforer() => View(new LoginVm());

        // Behandler innlogging for brukere med rollen "Registerforer"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registerforer(LoginVm vm)
        {
            // Valider inputmodell
            if (!ModelState.IsValid) return View(vm);

            // Finn bruker på e-post
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            // Sjekk at brukeren har rollen "Registerforer"
            if (!await _userManager.IsInRoleAsync(user, "Registerforer"))
            {
                ModelState.AddModelError("", "You are not a Registrar user.");
                return View(vm);
            }

            // Forsøk å logge inn med passord
            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            // Ved suksess sendes Registerfører til oversiktssiden for hindere
            return RedirectToAction("Overview", "Obstacle");
        }

        // -------- Admin-login --------
        
        // Viser eget innloggingsskjema for Admin-brukere
        [HttpGet]
        public IActionResult Admin() => View(new LoginVm());

        // Behandler innlogging for brukere med rollen "Admin"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin(LoginVm vm)
        {
            // Valider inputmodell
            if (!ModelState.IsValid) return View(vm);

            // Finn bruker på e-post
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            // Sjekk at brukeren har rollen "Admin"
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "You are not an Admin user.");
                return View(vm);
            }

            // Forsøk å logge inn med passord
            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(vm);
            }

            // Ved suksess sendes Admin til Admin-oversikten
            return RedirectToAction("Index", "Admin");
        }

        // Logger ut innlogget bruker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Fjern autentiseringscookie og annen innloggingsstatus
            await _signInManager.SignOutAsync();
            // Etter utlogging sendes man tilbake til standard login-side (Index)
            return RedirectToAction("Index");
        }

        // Valgfritt view som kan vises ved manglende tilgang (401/403)
        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
