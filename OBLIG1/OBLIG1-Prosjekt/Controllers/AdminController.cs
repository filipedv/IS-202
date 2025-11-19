using Microsoft.AspNetCore.Mvc;
using OBLIG1.Data;
using OBLIG1.Models;
using System.Linq;

namespace OBLIG1.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show all users
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users); // Views/Admin/Users.cshtml
        }

        // Add user - GET
        public IActionResult AddUser()
        {
            return View();
        }

        // Add user - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Users");
            }
            return View(user);
        }

        // Edit user - GET
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // Edit user - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                return RedirectToAction("Users");
            }
            return View(user);
        }

        // Block / Unblock user
        public IActionResult ToggleBlock(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            user.IsBlocked = !user.IsBlocked;
            _context.SaveChanges();
            return RedirectToAction("Users");
        }

        // Delete user
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Users");
        }
    }
}