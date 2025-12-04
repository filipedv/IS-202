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
            // Client-side validering for umiddelbar feedback
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
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "User authentication error. Please log in again.");
                return View(vm);
            }

            try
            {
                // Service vil også validere (server-side)
                await _obstacleService.CreateAsync(vm, userId);
                TempData["SuccessMessage"] = "Obstacle registered successfully.";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(vm);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the obstacle. Please try again.");
                return View(vm);
            }
        }

        // ---------- Overview ----------

        /// <summary>
        /// Viser liste over hindere. Registerfører ser alle, pilot ser egne.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            try
            {
                var list = await _obstacleService.GetOverviewAsync(User);
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while loading obstacles.";
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
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
                {
                    ModelState.AddModelError(string.Empty, "Obstacle not found.");
                    PopulateEditViewModelUi(vm);
                    return View(vm);
                }

                TempData["SuccessMessage"] = "Obstacle updated successfully.";
                return RedirectToAction(nameof(Overview));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the obstacle.");
                PopulateEditViewModelUi(vm);
                return View(vm);
            }
        }

        // ---------- Delete ----------

        /// <summary>
        /// Sletter et hinder. Pilot kan bare slette egne, registerfører kan slette alle.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _obstacleService.DeleteAsync(id, User);
                if (!deleted)
                {
                    TempData["ErrorMessage"] = "Obstacle not found.";
                    return RedirectToAction(nameof(Overview));
                }

                TempData["SuccessMessage"] = "Obstacle deleted successfully.";
                return RedirectToAction(nameof(Overview));
            }
            catch (UnauthorizedAccessException)
            {
                TempData["ErrorMessage"] = "You do not have permission to delete this obstacle.";
                return RedirectToAction(nameof(Overview));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the obstacle.";
                return RedirectToAction(nameof(Overview));
            }
        }
    }
}
