using Microsoft.EntityFrameworkCore;
using TravelMateApi.Models;

namespace TravelMateApi.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<DbJourney> Journeys { get; set; }
        public DbSet<DbLine> Lines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(Credentials.DbConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DbJourney>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Uid).IsRequired();
                entity.Property(e => e.Position).IsRequired();
                entity.Property(e => e.StartLocation).IsRequired();
                entity.Property(e => e.EndLocation).IsRequired();
            });

            modelBuilder.Entity<DbLine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LineId).IsRequired();
                entity.Property(e => e.Name).IsRequired();
            });
        }
    }
}