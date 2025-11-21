using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OBLIG1.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace OBLIG1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Dashboard: List all users
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // Add user - GET
        public IActionResult AddUser() => View();

        // Add user - POST
        [HttpPost]
        public async Task<IActionResult> AddUser(ApplicationUser user, string password)
        {
            if (!ModelState.IsValid) return View(user);

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
                return RedirectToAction("Users");

            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View(user);
        }

        // Edit user - GET
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // Edit user - POST
        [HttpPost]
        public async Task<IActionResult> EditUser(ApplicationUser updated)
        {
            var user = await _userManager.FindByIdAsync(updated.Id);
            if (user == null) return NotFound();

            user.Email = updated.Email;
            user.UserName = updated.UserName;
            user.Role = updated.Role;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Users");
        }

        // Block / Unblock
        public async Task<IActionResult> ToggleBlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsBlocked = !user.IsBlocked;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Users");
        }

        // Delete user
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction("Users");
        }
    }
}
