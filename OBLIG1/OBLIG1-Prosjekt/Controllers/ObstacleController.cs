using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    public class ObstacleController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ObstacleController(ApplicationDbContext db) => _db = db;

        // =========================
        // Create via map (DataForm)
        // =========================

        [HttpGet]
        public IActionResult DataForm() => View(new ObstacleData());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DataForm(ObstacleData vm)
        {
            if (string.IsNullOrWhiteSpace(vm.GeometryGeoJson))
            {
                ModelState.AddModelError(nameof(vm.GeometryGeoJson),
                    "Draw the obstacle on the map before submitting.");
                return View(vm);
            }

            var entity = new Obstacle
            {
                Name            = string.IsNullOrWhiteSpace(vm.ObstacleName) ? "Obstacle" : vm.ObstacleName,
                Height          = (vm.ObstacleHeight <= 0) ? null : vm.ObstacleHeight, // meter i DB (double?)
                Description     = vm.ObstacleDescription ?? string.Empty,
                IsDraft         = vm.IsDraft,
                Type            = null, // settes i Edit
                GeometryGeoJson = vm.GeometryGeoJson, // <-- VIKTIG: lagre geometri ved opprettelse
                RegisteredAt    = DateTime.UtcNow
            };

            _db.Obstacles.Add(entity);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Overview));
        }

        
        // ===============
        // Overview (Liste)
        // ===============

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var list = await _db.Obstacles
                .OrderByDescending(o => o.RegisteredAt)
                .ToListAsync();

            return View(list);
        }

        
        // =========
        // Edit (Form)
        // =========

        private static IEnumerable<SelectListItem> GetTypeOptions(string? current = null) =>
            new[]
            {
                new SelectListItem("Tower",    "Tower",    current == "Tower"),
                new SelectListItem("Crane",    "Crane",    current == "Crane"),
                new SelectListItem("Building", "Building", current == "Building"),
                new SelectListItem("Mast",     "Mast",     current == "Mast"),
                new SelectListItem("Other",    "Other",    current == "Other"),
            };

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var e = await _db.Obstacles.FindAsync(id);
            if (e == null) return NotFound();

            var vm = new ObstacleEditViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                HeightFt = e.Height.HasValue ? (int)Math.Round(e.Height.Value * 3.28084) : null, // m -> ft
                Type = e.Type,
                IsDraft = e.IsDraft,
                GeometryGeoJson = e.GeometryGeoJson, // <-- VIKTIG: send geometri til Edit-kartet
                TypeOptions = GetTypeOptions(e.Type)
            };

            return View(vm); // Views/Obstacle/Edit.cshtml (med kart)
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ObstacleEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.TypeOptions = GetTypeOptions(vm.Type);
                return View(vm);
            }

            var e = await _db.Obstacles.FindAsync(vm.Id);
            if (e == null) return NotFound();

            e.Name        = vm.Name;
            e.Description = vm.Description;
            e.IsDraft     = vm.IsDraft;
            e.Type        = vm.Type;
            e.Height      = vm.HeightFt.HasValue ? vm.HeightFt.Value / 3.28084 : null; // ft -> m

            e.GeometryGeoJson = vm.GeometryGeoJson; // <-- VIKTIG: lagre redigert geometri

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Overview));
        }
    }
}


