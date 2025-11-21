using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OBLIG1.Models;

namespace OBLIG1.Controllers;

//HomeController styrer trafikken på home-siden
//MVC-kontroller, håndterer http forespørsler og returnerer views(HTML-side)/JSON(data)
public class HomeController : Controller 
{
    //Logger som kan brukes til feilsøking
    private readonly ILogger<HomeController> _logger; 
    
    
    //Rammeverket som gir logging
    public HomeController(ILogger<HomeController> logger) 
    {
        _logger = logger;
    }
    
    //Peker på index.cshtml og returnerer den som et view i nettleseren
    public IActionResult Index()
    {
        return View();
    }
    
    //Peker på privacy.cshtml og returnerer den som et view i nettleseren
    public IActionResult Privacy()
    {
        return View();
    }
    
    //Viser en feilmeldingsside som aldri caches og har en unik ID slik at hver enkelt forespørsel- 
    //kan feilsøkes i logger
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}