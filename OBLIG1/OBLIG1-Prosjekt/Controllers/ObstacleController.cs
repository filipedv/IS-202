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
        
        //Viser kart-skjema for å registrere et nytt hinder.
        [HttpGet]
        public IActionResult DataForm() => View(new ObstacleData());
        
        // Tar imot innsending fra kart-skjema og oppretter et nytt hinder.
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
        
        // Viser liste over hindere. Registerfører ser alle, pilot ser egne.
        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var list = await _obstacleService.GetOverviewAsync(User);
            return View(list);
        }

        // ---------- Hjelpe-metoder for dropdowns (UI-lag) ----------
        
        // Lager liste over typer hindere til *dropdown
        //*Dropdown: et UI-element som lar brukeren velge ett alternativ fra en liste
        // Setter valgt verdi hvis oppgitt
        private static IEnumerable<SelectListItem> GetTypeOptions(string? current = null) =>
            new[]
            {
                new SelectListItem("Tower",    "Tower",    current == "Tower"),
                new SelectListItem("Crane",    "Crane",    current == "Crane"),
                new SelectListItem("Building", "Building", current == "Building"),
                new SelectListItem("Mast",     "Mast",     current == "Mast"),
                new SelectListItem("Other",    "Other",    current == "Other"),
            };
        // Lager liste over statusvalg til dropdown
        // Setter valgt status basert på current
        private static IEnumerable<SelectListItem> GetStatusOptions(ObstacleStatus current) =>
            new[]
            {
                new SelectListItem("Pending",  ObstacleStatus.Pending.ToString(),  current == ObstacleStatus.Pending),
                new SelectListItem("Approved", ObstacleStatus.Approved.ToString(), current == ObstacleStatus.Approved),
                new SelectListItem("Rejected", ObstacleStatus.Rejected.ToString(), current == ObstacleStatus.Rejected),
            };
        // Fyller skjemaet med type- og statusvalg, og setter om brukeren kan endre status
        private void PopulateEditViewModelUi(ObstacleEditViewModel vm)
        {
            vm.TypeOptions   = GetTypeOptions(vm.Type);
            vm.StatusOptions = GetStatusOptions(vm.Status);
            vm.CanEditStatus = User.IsInRole(AppRoles.Registrar);
        }

        // ---------- Edit (kart + metadata) ----------

        // Viser redigeringsskjema for et eksisterende hinder
        // Piloter kan kun redigere egne hindere
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
        
        // Tar imot redigering av hinder
        // Sjekker at alt er fylt ut og oppdaterer hinderet hvis brukeren har rettigheter
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
