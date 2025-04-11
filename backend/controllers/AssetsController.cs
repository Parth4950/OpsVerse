using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace OpsVerse.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public AssetsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet("{filename}")]
        public IActionResult GetAsset(string filename)
        {
            var path = Path.Combine(_environment.ContentRootPath, "Assets", filename);
            
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }
            
            var contentType = "application/octet-stream";
            if (filename.EndsWith(".png")) contentType = "image/png";
            if (filename.EndsWith(".jpg")) contentType = "image/jpeg";
            if (filename.EndsWith(".json")) contentType = "application/json";
            
            return PhysicalFile(path, contentType);
        }
    }
}