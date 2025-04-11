using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OpsVerse.Backend.Models
{
    public class Node
    {
        [Key]
        public Guid NodeId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "unknown";
        
        [Column(TypeName = "jsonb")]
        public object Metadata { get; set; }
        
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}