using Microsoft.AspNetCore.Mvc;

namespace OpsVerse.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> Get()
        {
            // Simple endpoint for Unity to test connectivity
            return new { status = "healthy", timestamp = DateTime.UtcNow };
        }
    }
}