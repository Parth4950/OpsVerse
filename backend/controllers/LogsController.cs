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
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemLog>>> GetLogs()
        {
            return await _context.SystemLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(100)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SystemLog>> GetLog(Guid id)
        {
            var log = await _context.SystemLogs.FindAsync(id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        // Add a new log entry
        [HttpPost]
        public async Task<ActionResult<SystemLog>> CreateLog(SystemLog log)
        {
            // Generate a new ID if not provided
            if (log.LogId == Guid.Empty)
            {
                log.LogId = Guid.NewGuid();
            }
            
            // Set timestamp to current time if not provided
            if (log.Timestamp == default)
            {
                log.Timestamp = DateTime.UtcNow;
            }
            
            _context.SystemLogs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLog), new { id = log.LogId }, log);
        }

        // Add multiple log entries in a single request (batch insert)
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<SystemLog>>> CreateLogs(List<SystemLog> logs)
        {
            foreach (var log in logs)
            {
                // Generate a new ID if not provided
                if (log.LogId == Guid.Empty)
                {
                    log.LogId = Guid.NewGuid();
                }
                
                // Set timestamp to current time if not provided
                if (log.Timestamp == default)
                {
                    log.Timestamp = DateTime.UtcNow;
                }
            }
            
            _context.SystemLogs.AddRange(logs);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLogs), logs);
        }
    }
}