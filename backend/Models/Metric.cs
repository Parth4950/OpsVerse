using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsVerse.Backend.Models
{
    public class Metric
    {
        [Key]
        public Guid MetricId { get; set; }
        
        [Required]
        public Guid NodeId { get; set; }
        
        [ForeignKey("NodeId")]
        public virtual Node Node { get; set; }
        
        [Required]
        [StringLength(255)]
        public string MetricName { get; set; }
        
        public float MetricValue { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}