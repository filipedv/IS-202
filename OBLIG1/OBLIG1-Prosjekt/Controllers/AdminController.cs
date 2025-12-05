using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Models;
using OBLIG1.ViewModels.Admin;

namespace OBLIG1.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Hjelpemetode: hent alle rollenavn sortert
        private Task<List<string>> GetAllRoleNamesAsync() =>
            _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToListAsync();

        // ---------- INDEX: liste alle brukere ----------

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            var list = new List<AdminUserListViewModel>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);

                list.Add(new AdminUserListViewModel
                {
                    Id    = u.Id,
                    Email = u.Email ?? "",
                    Roles = roles.ToList()
                });
            }

            return View(list);
        }

        // ---------- CREATE : Viser skjema og håndterer opprettelse av ny bruker på Admin side----------
        
        //Viser administasjonssiden for å opprette en bruker.
        //Initialiserer en ViewModel og henter tilgjengelige roller slik at 
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new AdminUserEditViewModel
            {
                AvailableRoles = await GetAllRoleNamesAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create(AdminUserEditViewModel vm)
        {
            // Sikrer at ViewModel følger valideringsreglene
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }
            // Manuell validering av passord (Passord kreves spesielt for nye brukere)
            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError(nameof(vm.Password), "Password is required.");
                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }
            // Kontroll for eksisterende bruker (Unngår at to kontoer opprettes med sammen e-postadresse)
            var existing = await _userManager.FindByEmailAsync(vm.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email is already in use.");
                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }
            // Opprettelse av bruker (Brukeren opprettes via UserManager, som håndterer lagring og hashing av passord)
            var user = new ApplicationUser
            {
                UserName       = vm.Email,
                Email          = vm.Email,
                EmailConfirmed = true
            };
            // Tildeling av rolle (Nye brukere knyttes til valgt rolle for tilgangsstyring)
            var createResult = await _userManager.CreateAsync(user, vm.Password);
            if (!createResult.Succeeded)
            {
                foreach (var e in createResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }
            // Redirect ved suksess (Brukeren sendes til Index-visningen for å forhindre dobbel innsending ved redirect)
            var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);
            if (!roleResult.Succeeded)
            {
                foreach (var e in roleResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- EDIT: Administrator kan redigere bruker informasjon ----------

        [HttpGet]
        public async Task<IActionResult> Edit(string id)  
        {
            // Hent bruker, return error hvis den ikke finnes
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Tilgjengelige roller 
            var roles     = await GetAllRoleNamesAsync();
            // Brukerens nåværigende rolle
            var userRoles = await _userManager.GetRolesAsync(user);

            // Map data til viewmodel som brukes i edit skjema
            var vm = new AdminUserEditViewModel
            {
                Id            = user.Id,
                Email         = user.Email ?? "",
                Role          = userRoles.FirstOrDefault() ?? "",
                AvailableRoles = roles
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //CSRF beskyttelse: post kommer fra eget skjema 
        public async Task<IActionResult> Edit(AdminUserEditViewModel vm)
        {
            //Validerer input på roles
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

            //Sikrer korrekte oppdatertinger
            var user = await _userManager.FindByIdAsync(vm.Id!); 
            if (user == null) return NotFound();
            // Oppdater e-post og brukernavn (brukernavn settes lik epost for testbrukere)
            user.Email    = vm.Email;
            user.UserName = vm.Email;

            // Oppdatere bruker i identiity database
            var updateResult = await _userManager.UpdateAsync(user); 
            if (!updateResult.Succeeded)
            {
                // Hvis oppdatering feiler blir det vist i view
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                // Fyll inn roller før view returneres
                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

            // Hent nåværende roller for brukeren
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                // Fjerner bruker fra eksisterende roller før en ny rolle blir lagt til
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Legger brukeren til i valgt rolle
            var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);
            if (!roleResult.Succeeded)
            {
                // Vis error hvis rolletildeling feiler
                foreach (var e in roleResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

            // Trygg passordendring
            if (!string.IsNullOrWhiteSpace(vm.Password)) 
            {
                var token      = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, vm.Password);
                if (!passResult.Succeeded)
                {
                    // Vis error hvis reset feiler
                    foreach (var e in passResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);

                    vm.AvailableRoles = await GetAllRoleNamesAsync();
                    return View(vm);
                }
            }

            // Alt ok -> redirect til index
            return RedirectToAction(nameof(Index));
        }

        // ---------- DELETE ----------
        // Sletter en bruker baser på ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Ikke la en admin slette seg selv mens han er innlogget
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "You cannot delete the user account you are currently logged in with.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
