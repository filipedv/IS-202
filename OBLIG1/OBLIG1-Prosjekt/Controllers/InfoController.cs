using Microsoft.AspNetCore.Mvc;

namespace OBLIG1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfoController : ControllerBase
{
    [HttpGet("servertime")]
    public IActionResult ServerTime() => Ok(new { serverTime = DateTimeOffset.UtcNow });
}