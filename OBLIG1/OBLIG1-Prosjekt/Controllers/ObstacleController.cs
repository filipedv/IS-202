// Håndterer registrering og visning av hindringer og lagrer midlertidig i minnet

using Microsoft.AspNetCore.Mvc;
using OBLIG1.Models;
using System.Collections.Generic;

namespace OBLIG1.Controllers
{
    public class ObstacleController : Controller
    {
        // Lagre alle innsendte hindringer i minnet
        private static List<ObstacleData> obstacles = new List<ObstacleData>();

        // GET: vis registreringsskjemaet
        [HttpGet]
        public ActionResult DataForm()
        {
            return View();
        }

        // POST: håndtere innsending av registreringsskjemaet
        [HttpPost]
        public ActionResult DataForm(ObstacleData obstacledata)
        {
            if (obstacledata != null)
            {
                obstacles.Add(obstacledata); // Lagre innsendt hindring
            }

            // Valgfritt: Sjekk for utkast
            bool isDraft = string.IsNullOrEmpty(obstacledata?.ObstacleDescription);

            // Gå til Overview for å vise alle hindringer
            return RedirectToAction("Overview");
        }

        // GET: vise oversikt over alle hinderinger
        [HttpGet]
        public ActionResult Overview()
        {
            return View(obstacles); // Sende liste til visning
        }
    }
}