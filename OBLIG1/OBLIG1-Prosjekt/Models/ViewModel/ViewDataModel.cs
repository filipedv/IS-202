using Microsoft.AspNetCore.Mvc;

namespace OBLIG1.Controllers

public class DataController : Controller

[HttpGet]
public async Task<IActionResult> ShowData()
{
    return View();
}
        
[HttpPost] //Det som sendes til brukeren?
public async Task<IActionResult> ShowData()
{
    return View();
}