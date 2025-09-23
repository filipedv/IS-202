using Microsoft.AspNetCore.Mvc;
using OBLIG1.Models;

namespace OBLIG1.Controllers;

public class ObstacleController : Controller
{
    // blir kalt etter at vi trykker på "Register Obstacle" lenken i Index viewet
    [HttpGet]
    public ActionResult DataForm()
    {
        return View();
    }


    // blir kalt etter at vi trykker på "Submit Data" knapp i DataForm viewet
    [HttpPost]
    public ActionResult DataForm(ObstacleData obstacledata)
    {
        bool isDraft = false;
        if (obstacledata.ObstacleDescription == null)
        { 
            isDraft = true;
        }

        return View("Overview", obstacledata);
    }
}