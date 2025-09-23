using Microsoft.AspNetCore.Mvc;
using OBLIG1.Models;
using OBLIG1.Services;

namespace OBLIG1.Controllers;

public class PointsController : Controller
{
    private readonly IPointRepository _repo;
    public PointsController(IPointRepository repo) => _repo = repo;

    [HttpGet]
    public IActionResult Index() => View(_repo.All());

    [HttpGet]
    public IActionResult Create() => View(new PointInputModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PointInputModel input)
    {
        if (!ModelState.IsValid) return View(input);

        var p = new Point
        {
            Title = input.Title,
            Latitude = input.Latitude,
            Longitude = input.Longitude
        };
        _repo.Add(p);

        TempData["Flash"] = "Punkt lagret!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Map() => View(_repo.All());
}