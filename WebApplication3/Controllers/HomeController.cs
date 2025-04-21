using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: api/home
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Message = "API is running" });
        }
    }
}