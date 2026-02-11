using Microsoft.AspNetCore.Mvc;

namespace MusicBares.Api.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("Backend vivo");
        }
    }
}
