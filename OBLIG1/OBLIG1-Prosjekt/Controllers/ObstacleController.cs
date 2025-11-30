using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBLIG1.Data;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    // Kun innloggede brukere i én av disse rollene får tilgang
    [Authorize(Roles = "Pilot,Registerforer")]
    public class ObstacleController : Controller
    {
        // Databasekontekst – brukes til å lese/lagre Obstacle-objekter
        private readonly ApplicationDbContext _db;
        
        // Konstruktør hvor ApplicationDbContext injiseres via dependency injection
        public ObstacleController(ApplicationDbContext db) => _db = db;

        // ---------- DataForm (Create via kart) ----------

        // Viser tomt skjema for å registrere et nytt hinder via kart (ObstacleData ViewModel)
        [HttpGet]
        public IActionResult DataForm() => View(new ObstacleData());

        // Tar imot innsending fra DataForm og oppretter et nytt hinder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DataForm(ObstacleData vm)
        {
            if (string.IsNullOrWhiteSpace(vm.GeometryGeoJson))
            {
                ModelState.AddModelError(nameof(vm.GeometryGeoJson),
                    "Draw the obstacle on the map before submitting.");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var entity = new Obstacle
            {
                Name            = string.IsNullOrWhiteSpace(vm.ObstacleName) ? "Obstacle" : vm.ObstacleName,
                Height          = (vm.ObstacleHeight <= 0) ? null : vm.ObstacleHeight,
                Description     = vm.ObstacleDescription ?? string.Empty,
                Type            = null,
                GeometryGeoJson = vm.GeometryGeoJson,
                RegisteredAt    = DateTime.UtcNow,
                CreatedByUserId = userId,
                Status          = ObstacleStatus.Pending
            };

            _db.Obstacles.Add(entity);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        // ---------- Overview (rollebasert datascope) ----------

        // Viser en oversikt over hindere – innholdet avhenger av brukerens rolle
        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            // Startspørring: alle hindere, sortert på nyeste først
            IQueryable<Obstacle> q = _db.Obstacles
                .OrderByDescending(o => o.RegisteredAt);

            if (User.IsInRole("Registerforer"))
            {
                // Registrar ser alle hindere
            }
            else
            {
                // Pilot: bare egne
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                q = q.Where(o => o.CreatedByUserId == userId);
            }

            // Gjør om spørring til liste og sender det til view
            var list = await q.ToListAsync();
            return View(list);
        }

        // ---------- Edit (kart + metadata) ----------

        // Dropdown for "Obstacle type"
        private static IEnumerable<SelectListItem> GetTypeOptions(string? current = null) =>
            new[]
            {
                new SelectListItem("Tower",    "Tower",    current == "Tower"),
                new SelectListItem("Crane",    "Crane",    current == "Crane"),
                new SelectListItem("Building", "Building", current == "Building"),
                new SelectListItem("Mast",     "Mast",     current == "Mast"),
                new SelectListItem("Other",    "Other",    current == "Other"),
            };

        // Dropdown for "Status"
        private static IEnumerable<SelectListItem> GetStatusOptions(ObstacleStatus current) =>
            new[]
            {
                new SelectListItem("Pending",  ObstacleStatus.Pending.ToString(),  current == ObstacleStatus.Pending),
                new SelectListItem("Approved", ObstacleStatus.Approved.ToString(), current == ObstacleStatus.Approved),
                new SelectListItem("Rejected", ObstacleStatus.Rejected.ToString(), current == ObstacleStatus.Rejected),
            };

        // Viser edit-skjema for et hinder, inkludert kart og metadata
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Finn hinderet i databasen    
            var e = await _db.Obstacles.FindAsync(id);
            if (e == null) return NotFound();

            // Pilot kan bare redigere egne hindere
            if (!User.IsInRole("Registerforer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (e.CreatedByUserId != userId)
                    return Forbid();
            }

            // Hent e-post (eller brukernavn) til den som registrerte hinderet
            var createdByEmail = await _db.Users
                .Where(u => u.Id == e.CreatedByUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            // Bygg ViewModel for redigering
            var vm = new ObstacleEditViewModel
            {
                Id          = e.Id,
                Name        = e.Name,
                Description = e.Description,
                HeightFt    = e.Height.HasValue
                    ? (int)Math.Round(e.Height.Value * 3.28084)
                    : null,
                Type            = e.Type,
                GeometryGeoJson = e.GeometryGeoJson,
                TypeOptions     = GetTypeOptions(e.Type),

                // Status-info til view
                Status        = e.Status,
                StatusOptions = GetStatusOptions(e.Status),
                CanEditStatus = User.IsInRole("Registerforer"),

                CreatedByUser = createdByEmail ?? "(unknown)"
            };

            return View(vm);
        }


        // Behandler innsending av edit-skjema (lagring av endringer)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ObstacleEditViewModel vm)
        {
            // Ved valideringsfeil må vi fylle opp dropdown-lister på nytt og vise viewet igjen
            if (!ModelState.IsValid)
            {
                vm.TypeOptions     = GetTypeOptions(vm.Type);
                vm.StatusOptions   = GetStatusOptions(vm.Status);
                vm.CanEditStatus   = User.IsInRole("Registerforer");
                return View(vm);
            }

            // Finn eksisterende hinder i databasen
            var e = await _db.Obstacles.FindAsync(vm.Id);
            if (e == null) return NotFound();

            // Pilot kan bare lagre egne hindere
            if (!User.IsInRole("Registerforer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (e.CreatedByUserId != userId)
                    return Forbid();
            }

            e.Name        = vm.Name;
            e.Description = vm.Description;
            e.Type        = vm.Type;
            e.Height      = vm.HeightFt.HasValue
                                ? vm.HeightFt.Value / 3.28084
                                : null;
            e.GeometryGeoJson = vm.GeometryGeoJson;

            // Kun registerfører kan endre status
            if (User.IsInRole("Registerforer"))
                e.Status = vm.Status;

            // Lagre endringer til databasen
            await _db.SaveChangesAsync();
            // Etter lagring sendes bruker tilbake til oversiktssiden
            return RedirectToAction(nameof(Overview));
        }
    }
}


