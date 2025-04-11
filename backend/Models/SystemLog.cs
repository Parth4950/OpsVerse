using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsVerse.Backend.Models
{
    public class SystemLog
    {
        [Key]
        public Guid LogId { get; set; }
        
        public Guid? NodeId { get; set; }
        
        [ForeignKey("NodeId")]
        public virtual Node Node { get; set; }
        
        [Required]
        [StringLength(20)]
        public string LogLevel { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "jsonb")]
        public object Metadata { get; set; }
    }
}