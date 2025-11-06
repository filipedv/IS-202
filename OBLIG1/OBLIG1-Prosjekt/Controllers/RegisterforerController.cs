using Microsoft.AspNetCore.Mvc;

namespace OBLIG1.Controllers
{

    public class RegisterforerController : Controller
    {
        public IActionResult Index()
        {
            // Returner view med data om registrerte hinder
            return View();
        }
    }
}
