using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    // Kun Admin skal inn her
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // --- ViewModels kun for admin-sidene ---

        public class AdminUserListViewModel
        {
            public string Id { get; set; } = "";
            public string Email { get; set; } = "";
            public IList<string> Roles { get; set; } = new List<string>();
        }

        public class AdminUserEditViewModel
        {
            public string? Id { get; set; }

            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.EmailAddress]
            public string Email { get; set; } = "";

            [System.ComponentModel.DataAnnotations.DataType(
                System.ComponentModel.DataAnnotations.DataType.Password)]
            public string? Password { get; set; }   // påkrevd på Create, valgfritt på Edit

            [System.ComponentModel.DataAnnotations.Required]
            public string Role { get; set; } = "";

            public List<string> AvailableRoles { get; set; } = new();
        }

        // --- fields ---

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

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

        // ---------- CREATE: ny bruker ----------

        [HttpGet]
        public async Task<IActionResult> Create()
        {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminUserEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError(nameof(vm.Password), "Password is required.");
                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

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

            var user = new ApplicationUser
            {
                UserName       = vm.Email,
                Email          = vm.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, vm.Password);
            if (!createResult.Succeeded)
            {
                foreach (var e in createResult.Errors)
                    ModelState.AddModelError("", e.Description);

                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

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

            return RedirectToAction(nameof(Index));
        }

        // ---------- EDIT: rediger eksisterende bruker ----------

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(n => n)
                .ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(user);

            var vm = new AdminUserEditViewModel
            {
                Id    = user.Id,
                Email = user.Email ?? "",
                Role  = userRoles.FirstOrDefault() ?? "",
                AvailableRoles = roles
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = await _roleManager.Roles
                    .Select(r => r.Name!)
                    .OrderBy(n => n)
                    .ToListAsync();
                return View(vm);
            }

            var user = await _userManager.FindByIdAsync(vm.Id!);
            if (user == null) return NotFound();

            user.Email    = vm.Email;
            user.UserName = vm.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
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
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

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

            // Endre passord hvis fylt ut
            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
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

            return RedirectToAction(nameof(Index));
        }

        // ---------- DELETE: slett bruker ----------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
