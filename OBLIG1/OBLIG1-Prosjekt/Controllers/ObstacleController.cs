using Microsoft.AspNetCore.Mvc;
using OBLIG1.Models;
using System.Collections.Generic;

namespace OBLIG1.Controllers
{
    public class ObstacleController : Controller
    {
        // Store all submitted obstacles in-memory
        private static List<ObstacleData> obstacles = new List<ObstacleData>();

        // GET: show the form
        [HttpGet]
        public ActionResult DataForm()
        {
            return View();
        }

        // POST: process form submission
        [HttpPost]
        public ActionResult DataForm(ObstacleData obstacledata)
        {
            if (obstacledata != null)
            {
                obstacles.Add(obstacledata); // store submitted obstacle
            }

            // Optional: check for draft
            bool isDraft = string.IsNullOrEmpty(obstacledata?.ObstacleDescription);

            // Redirect to Overview to display all obstacles
            return RedirectToAction("Overview");
        }

        // GET: show overview of all obstacles
        [HttpGet]
        public ActionResult Overview()
        {
            return View(obstacles); // pass list to view
        }
    }
}