using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    // Kun brukere med rollen "Admin" har tilgang til denne kontrolleren
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // --- ViewModels kun for admin-sidene ---
        
        // ViewModel for listevisning av brukere i admin-oversikten
        public class AdminUserListViewModel
        {
            // Brukerens Id i Identity-systemet
            public string Id { get; set; } = "";
           
            // Brukerens e-post (viser også brukernavn, siden de ofte er like)
            public string Email { get; set; } = "";
           
            // Roller brukeren tilhører
            public IList<string> Roles { get; set; } = new List<string>();
        } 
        
        // ViewModel for oppretting og redigering av en bruker
        public class AdminUserEditViewModel
        {
            // Brukerens Id (null når vi oppretter en ny)
            public string? Id { get; set; } 
            
            // Påkrevd felt for e-post, med validering
            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.EmailAddress]
            public string Email { get; set; } = ""; 
            
            // Passord (påkrevd ved Create, valgfritt ved Edit)
            [System.ComponentModel.DataAnnotations.DataType(
                System.ComponentModel.DataAnnotations.DataType.Password)]
            public string? Password { get; set; }   // påkrevd på Create, valgfritt på Edit
                                                    
            // Påkrevd én rolle per bruker (hovedrolle)
            [System.ComponentModel.DataAnnotations.Required]
            public string Role { get; set; } = ""; 
            
            // Liste over alle roller som kan velges i UI
            public List<string> AvailableRoles { get; set; } = new();
        }

        // --- fields ---
        
        // Håndterer brukere (oppretting, oppdatering, sletting, roller, passord osv.)
        private readonly UserManager<ApplicationUser> _userManager;
        // Håndterer roller (opprette roller, liste roller osv.)
        private readonly RoleManager<IdentityRole> _roleManager;
        
        // Konstruktør hvor UserManager og RoleManager injiseres via dependency injection
        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ---------- INDEX: liste alle brukere ----------

        // Viser en liste over alle brukere med tilhørende roller
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Hent alle brukere sortert på e-post
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var list = new List<AdminUserListViewModel>();
            
            // Bygg opp en liste med ViewModel som også inneholder roller
            foreach (var u in users)
            {
                // Hent rollene til hver enkelt bruker
                var roles = await _userManager.GetRolesAsync(u);

                list.Add(new AdminUserListViewModel
                {
                    Id    = u.Id,
                    Email = u.Email ?? "",
                    Roles = roles.ToList()
                });
            }
            // Send listen til viewet
            return View(list);
        }

        // ---------- CREATE: ny bruker ----------
        
        // Viser skjema for å opprette en ny bruker
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Hent alle tilgjengelige roller for å vise i nedtrekksliste
            var roles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToListAsync();

            var vm = new AdminUserEditViewModel
            {
                AvailableRoles = roles
            };

            return View(vm);
        }
        // Behandler innsending av skjemaet for å opprette en ny bruker
        [HttpPost]
        [ValidateAntiForgeryToken] // Beskyttelse mot CSRF-angrep
        public async Task<IActionResult> Create(AdminUserEditViewModel vm)
        {
            // Server-side validering av modell
            if (!ModelState.IsValid)
            {
                // Vi må fylle AvailableRoles på nytt når vi returnerer viewet
                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            // Sjekk at passord faktisk er oppgitt ved oppretting
            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError(nameof(vm.Password), "Password is required.");
                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            // Sjekk om det allerede finnes en bruker med samme e-post
            var existing = await _userManager.FindByEmailAsync(vm.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email is already in use.");
                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            // Opprett en ny ApplicationUser basert på input
            var user = new ApplicationUser
            {
                UserName       = vm.Email,
                Email          = vm.Email,
                EmailConfirmed = true // Hopper over e-postbekreftelse for admin-opprettede brukere
            };

            // Forsøk å opprette brukeren med gitt passord
            var createResult = await _userManager.CreateAsync(user, vm.Password);
            if (!createResult.Succeeded)
            {
                // Hvis opprettelse feiler, legg til feilmeldinger i ModelState
                foreach (var e in createResult.Errors)
                    ModelState.AddModelError("", e.Description);

                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            // Legg brukeren til valgt rolle
            var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);
            if (!roleResult.Succeeded)
            {
                // Hvis tildeling av rolle feiler, vis feilmeldinger
                foreach (var e in roleResult.Errors)
                    ModelState.AddModelError("", e.Description);

                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            // Alt gikk bra, gå tilbake til oversikten
            return RedirectToAction(nameof(Index));
        }

        // ---------- EDIT: rediger eksisterende bruker ----------

        // Viser skjema for å redigere en eksisterende bruker
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            // Finn bruker basert på Id
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Hent alle tilgjengelige roller
            var roles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToListAsync();

            // Hent brukerens nåværende roller
            var userRoles = await _userManager.GetRolesAsync(user);

            // Bygg ViewModel med aktuell informasjon
            var vm = new AdminUserEditViewModel
            {
                Id    = user.Id,
                Email = user.Email ?? "",
                // Antar at brukeren har én hovedrolle; velger den første
                Role  = userRoles.FirstOrDefault() ?? "",
                AvailableRoles = roles
            };

            return View(vm);
        }

        // Behandler innsending av skjema for å redigere bruker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserEditViewModel vm)
        {
            // Valider input
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            // Finn brukeren som skal oppdateres
            var user = await _userManager.FindByIdAsync(vm.Id!);
            if (user == null) return NotFound();

            // Oppdater e-post og brukernavn
            user.Email    = vm.Email;
            user.UserName = vm.Email;

            // Lagre endringer på brukeren
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                // Vis eventuelle feil ved oppdatering
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError("", e.Description);

                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            // Oppdater roller (vi antar én "hovedrolle")
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                // Fjern alle eksisterende roller før vi legger til den nye
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Legg brukeren til valgt rolle
            var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);
            if (!roleResult.Succeeded)
            {
                foreach (var e in roleResult.Errors)
                    ModelState.AddModelError("", e.Description);

                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            // Endre passord hvis feltet er fylt ut
            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                // Generer reset-token og bytt passord uten å kjenne det gamle
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, vm.Password);
                if (!passResult.Succeeded)
                {
                    foreach (var e in passResult.Errors)
                        ModelState.AddModelError("", e.Description);

                    vm.AvailableRoles = await _roleManager.Roles
                        .Select(r => r.Name!)
                        .OrderBy(n => n)
                        .ToListAsync();
                    return View(vm);
                }
            }

            // Tilbake til oversikten hvis alt gikk bra
            return RedirectToAction(nameof(Index));
        }

        // ---------- DELETE: slett bruker ----------

        // Sletter en bruker basert på Id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            // Finn brukeren som skal slettes
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Utfør sletting
            await _userManager.DeleteAsync(user);
            // Redirect til oversikten etter sletting
            return RedirectToAction(nameof(Index));
        }
    }
}
