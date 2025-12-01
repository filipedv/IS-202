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

        // ---------- CREATE ----------

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
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError(nameof(vm.Password), "Password is required.");
                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

            var existing = await _userManager.FindByEmailAsync(vm.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(vm.Email), "Email is already in use.");
                vm.AvailableRoles = await GetAllRoleNamesAsync();
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
                    ModelState.AddModelError(string.Empty, e.Description);

                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

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

        // ---------- EDIT ----------

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles     = await GetAllRoleNamesAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableRoles = await GetAllRoleNamesAsync();
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
                    ModelState.AddModelError(string.Empty, e.Description);

                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, vm.Role);
            if (!roleResult.Succeeded)
            {
                foreach (var e in roleResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                vm.AvailableRoles = await GetAllRoleNamesAsync();
                return View(vm);
            }

            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                var token      = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, vm.Password);
                if (!passResult.Succeeded)
                {
                    foreach (var e in passResult.Errors)
                        ModelState.AddModelError(string.Empty, e.Description);

                    vm.AvailableRoles = await GetAllRoleNamesAsync();
                    return View(vm);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- DELETE ----------

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
