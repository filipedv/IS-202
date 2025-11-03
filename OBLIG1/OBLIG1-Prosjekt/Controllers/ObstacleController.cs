using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    public class ObstacleController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ObstacleController(ApplicationDbContext db) => _db = db;

        // GET: /Obstacle/DataForm
        [HttpGet]
        public IActionResult DataForm() => View(new ObstacleData());

        // POST: /Obstacle/DataForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DataForm(ObstacleData vm)
        {
            // Map-only: vi krever minst geometri
            if (string.IsNullOrWhiteSpace(vm.GeometryGeoJson))
            {
                ModelState.AddModelError(nameof(vm.GeometryGeoJson),
                    "Draw the obstacle on the map before submitting.");
                return View(vm);
            }

            // Sett fornuftige defaults for felter som ikke sendes i dette skjemaet
            var entity = new Obstacle
            {
                Name            = string.IsNullOrWhiteSpace(vm.ObstacleName) ? "Obstacle" : vm.ObstacleName,
                Height          = vm.ObstacleHeight <= 0 ? 0 : vm.ObstacleHeight,
                Description     = vm.ObstacleDescription ?? "",
                IsDraft         = vm.IsDraft,
                GeometryGeoJson = vm.GeometryGeoJson,
                RegisteredAt    = DateTime.UtcNow
            };

            _db.Obstacles.Add(entity);
            await _db.SaveChangesAsync();

            // ← Send brukeren tilbake til forsiden
            return RedirectToAction("Index", "Home");
        }

        // GET: /Obstacle/Overview
        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var list = await _db.Obstacles
                .OrderByDescending(o => o.RegisteredAt)
                .ToListAsync();
            return View(list);
        }
    }
}