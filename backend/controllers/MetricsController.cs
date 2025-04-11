using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpsVerse.Backend.Models;

namespace OpsVerse.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MetricsController(ApplicationDbContext context)
        {
            _context = context;
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
    }
}