using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpsVerse.Backend.Models;
using System.Linq;

namespace OpsVerse.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NodesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Node>>> GetNodes()
        {
            return await _context.Nodes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Node>> GetNode(Guid id)
        {
            var node = await _context.Nodes.FindAsync(id);

            if (node == null)
            {
                return NotFound();
            }

            return node;
        }

        [HttpGet("{id}/metrics")]
        public async Task<ActionResult<IEnumerable<Metric>>> GetNodeMetrics(Guid id)
        {
            return await _context.Metrics
                .Where(m => m.NodeId == id)
                .OrderByDescending(m => m.Timestamp)
                .Take(100)
                .ToListAsync();
        }

        // Add a new node
        [HttpPost]
        public async Task<ActionResult<Node>> CreateNode(Node node)
        {
            // Generate a new ID if not provided
            if (node.NodeId == Guid.Empty)
            {
                node.NodeId = Guid.NewGuid();
            }
            
            _context.Nodes.Add(node);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNode), new { id = node.NodeId }, node);
        }

        // Update an existing node
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNode(Guid id, Node node)
        {
            if (id != node.NodeId)
            {
                return BadRequest();
            }

            _context.Entry(node).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NodeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool NodeExists(Guid id)
        {
            return _context.Nodes.Any(e => e.NodeId == id);
        }
    }
}