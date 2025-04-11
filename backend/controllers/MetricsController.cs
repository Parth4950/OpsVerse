using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpsVerse.Backend.Models;
using Microsoft.Extensions.Logging;

namespace OpsVerse.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(ApplicationDbContext context, ILogger<MetricsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Metric>>> GetMetrics()
        {
            return await _context.Metrics.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Metric>> GetMetric(Guid id)
        {
            var metric = await _context.Metrics.FindAsync(id);

            if (metric == null)
            {
                return NotFound();
            }

            return metric;
        }

        // Add a new metric
        [HttpPost]
        public async Task<ActionResult<Metric>> CreateMetric(Metric metric)
        {
            // Generate a new ID if not provided
            if (metric.MetricId == Guid.Empty)
            {
                metric.MetricId = Guid.NewGuid();
            }
            
            // Set timestamp to current time if not provided
            if (metric.Timestamp == default)
            {
                metric.Timestamp = DateTime.UtcNow;
            }
            
            _context.Metrics.Add(metric);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMetric), new { id = metric.MetricId }, metric);
        }

        // Add multiple metrics in a single request (batch insert)
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Metric>>> CreateMetrics(List<Metric> metrics)
        {
            foreach (var metric in metrics)
            {
                // Generate a new ID if not provided
                if (metric.MetricId == Guid.Empty)
                {
                    metric.MetricId = Guid.NewGuid();
                }
                
                // Set timestamp to current time if not provided
                if (metric.Timestamp == default)
                {
                    metric.Timestamp = DateTime.UtcNow;
                }
            }
            
            _context.Metrics.AddRange(metrics);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMetrics), metrics);
        }

        [HttpPost("nodes/{nodeId}")]
        public async Task<IActionResult> UpdateMetrics(Guid nodeId, [FromBody] MetricUpdateDto metrics)
        {
            try
            {
                var node = await _context.Nodes.FindAsync(nodeId);
                if (node == null)
                {
                    return NotFound($"Node with ID {nodeId} not found");
                }

                // Update node metadata
                node.Metadata = new
                {
                    cpu_usage = metrics.CpuUsage,
                    memory_usage = metrics.MemoryUsage
                };
                node.UpdatedAt = DateTime.UtcNow;

                // Add new metric record
                var metric = new Metric
                {
                    NodeId = nodeId,
                    MetricName = "performance",
                    MetricValue = metrics.CpuUsage
                };
                _context.Metrics.Add(metric);

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating metrics for node {NodeId}", nodeId);
                return StatusCode(500, "An error occurred while updating metrics");
            }
        }

        [HttpGet("nodes/{nodeId}/history")]
        public async Task<IActionResult> GetMetricHistory(Guid nodeId, [FromQuery] int hours = 24)
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-hours);
                var metrics = await _context.Metrics
                    .Where(m => m.NodeId == nodeId && m.Timestamp >= cutoffTime)
                    .OrderBy(m => m.Timestamp)
                    .ToListAsync();

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving metric history for node {NodeId}", nodeId);
                return StatusCode(500, "An error occurred while retrieving metric history");
            }
        }
    }

    public class MetricUpdateDto
    {
        public float CpuUsage { get; set; }
        public float MemoryUsage { get; set; }
    }
}