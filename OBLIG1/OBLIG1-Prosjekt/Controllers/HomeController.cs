using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OBLIG1.Models;

namespace OBLIG1.Controllers
{
    // Krever at brukeren er logget inn som Pilot eller Registrar
    [Authorize(Roles = $"{AppRoles.Pilot},{AppRoles.Registrar}")]
    public class HomeController : Controller 
    {
        private readonly ILogger<HomeController> _logger; 
        
        public HomeController(ILogger<HomeController> logger) 
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        
        // Viser feilmeldingsside – tillat anonym slik at den også funker ved auth-feil
        // ResponseCache lagrer ikke cache, man får alltid en oppdatert feilmelding
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Unhandled error, RequestId={RequestId}", requestId);

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}