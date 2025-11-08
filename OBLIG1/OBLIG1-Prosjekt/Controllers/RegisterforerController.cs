using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    public class RegisterforerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterforerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Henter alle hindringer fra databasen
            var obstacles = _context.Obstacles.ToList();

            // Send listen til viewet
            return View(obstacles);
        }
    }
}

