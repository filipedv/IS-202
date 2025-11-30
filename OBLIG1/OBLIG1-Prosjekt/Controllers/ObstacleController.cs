using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OBLIG1.Models;
using OBLIG1.Services;

namespace OBLIG1.Controllers
{
    // Kun innloggede brukere i én av disse rollene får tilgang
    [Authorize(Roles = $"{AppRoles.Pilot},{AppRoles.Registrar}")]
    public class ObstacleController : Controller
    {
        private readonly IObstacleService _obstacleService;

        public ObstacleController(IObstacleService obstacleService)
        {
            _obstacleService = obstacleService;
        }

        // ---------- DataForm (Create via kart) ----------

        /// <summary>
        /// Viser kart-skjema for å registrere et nytt hinder.
        /// </summary>
        [HttpGet]
        public IActionResult DataForm() => View(new ObstacleData());

        /// <summary>
        /// Tar imot innsending fra kart-skjema og oppretter et nytt hinder.
        /// </summary>
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
            await _obstacleService.CreateAsync(vm, userId);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // ---------- Overview ----------

        /// <summary>
        /// Viser liste over hindere. Registerfører ser alle, pilot ser egne.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var list = await _obstacleService.GetOverviewAsync(User);
            return View(list);
        }

        // ---------- Hjelpe-metoder for dropdowns (UI-lag) ----------

        private static IEnumerable<SelectListItem> GetTypeOptions(string? current = null) =>
            new[]
            {
                new SelectListItem("Tower",    "Tower",    current == "Tower"),
                new SelectListItem("Crane",    "Crane",    current == "Crane"),
                new SelectListItem("Building", "Building", current == "Building"),
                new SelectListItem("Mast",     "Mast",     current == "Mast"),
                new SelectListItem("Other",    "Other",    current == "Other"),
            };

        private static IEnumerable<SelectListItem> GetStatusOptions(ObstacleStatus current) =>
            new[]
            {
                new SelectListItem("Pending",  ObstacleStatus.Pending.ToString(),  current == ObstacleStatus.Pending),
                new SelectListItem("Approved", ObstacleStatus.Approved.ToString(), current == ObstacleStatus.Approved),
                new SelectListItem("Rejected", ObstacleStatus.Rejected.ToString(), current == ObstacleStatus.Rejected),
            };

        private void PopulateEditViewModelUi(ObstacleEditViewModel vm)
        {
            vm.TypeOptions   = GetTypeOptions(vm.Type);
            vm.StatusOptions = GetStatusOptions(vm.Status);
            vm.CanEditStatus = User.IsInRole(AppRoles.Registrar);
        }

        // ---------- Edit (kart + metadata) ----------

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vm = await _obstacleService.GetEditViewModelAsync(id, User);
                if (vm == null)
                    return NotFound();

                PopulateEditViewModelUi(vm);
                return View(vm);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ObstacleEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                PopulateEditViewModelUi(vm);
                return View(vm);
            }

            try
            {
                var updated = await _obstacleService.UpdateAsync(vm, User);
                if (!updated)
                    return NotFound();

                return RedirectToAction(nameof(Overview));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
