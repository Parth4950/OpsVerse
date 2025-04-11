using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpsVerse.Backend.Models
{
    public class Connection
    {
        [Key]
        public Guid ConnectionId { get; set; }
        
        [Required]
        public Guid SourceNodeId { get; set; }
        
        [Required]
        public Guid TargetNodeId { get; set; }
        
        [StringLength(50)]
        public string Type { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "unknown";
        
        [Column(TypeName = "jsonb")]
        public object Metadata { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}