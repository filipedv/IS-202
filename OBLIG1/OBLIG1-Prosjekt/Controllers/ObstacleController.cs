using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;     // DbContext
using OBLIG1.Models;   // Obstacle + ObstacleData

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
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DataForm(ObstacleData vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Map fra view-model (ObstacleData) til entity (Obstacle)
            var entity = new Obstacle
            {
                Name = vm.ObstacleName,
                Height = vm.ObstacleHeight,
                Description = vm.ObstacleDescription,
                IsDraft = vm.IsDraft,
                GeometryGeoJson = vm.GeometryGeoJson,
                RegisteredAt = DateTime.UtcNow
            };

            _db.Obstacles.Add(entity);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Overview));
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