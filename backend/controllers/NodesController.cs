using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpsVerse.Backend.Data;
using OpsVerse.Backend.Models;
using System.Threading.Tasks;

namespace OpsVerse.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NodesController> _logger;

        public NodesController(ApplicationDbContext context, ILogger<NodesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetNodes([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            try
            {
                var nodes = await _context.Nodes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new { nodes = nodes, page = page, pageSize = pageSize };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving nodes");
                return StatusCode(500, "An error occurred while retrieving nodes");
            }
        }

        // Add other CRUD operations...
    }
}