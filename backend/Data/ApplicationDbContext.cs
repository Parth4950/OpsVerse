using Microsoft.EntityFrameworkCore;
using OpsVerse.Backend.Models;

namespace OpsVerse.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Metric> Metrics { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<VRSession> VRSessions { get; set; }
        public DbSet<Annotation> Annotations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Connection>()
                .HasOne<Node>()
                .WithMany()
                .HasForeignKey(c => c.SourceNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Connection>()
                .HasOne<Node>()
                .WithMany()
                .HasForeignKey(c => c.TargetNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Metric>()
                .HasOne<Node>()
                .WithMany()
                .HasForeignKey(m => m.NodeId);

            // Configure JSON columns
            modelBuilder.Entity<Node>()
                .Property(n => n.Metadata)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Connection>()
                .Property(c => c.Metadata)
                .HasColumnType("jsonb");

            modelBuilder.Entity<SystemLog>()
                .Property(l => l.Metadata)
                .HasColumnType("jsonb");
        }
    }
}