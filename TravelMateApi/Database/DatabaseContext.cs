using Microsoft.EntityFrameworkCore;
using TravelMateApi.Models;

namespace TravelMateApi.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<DbJourney> Journeys { get; set; }
        public DbSet<DbLine> Lines { get; set; }
        public DbSet<DbAccount> Accounts { get; set; }
        public DbSet<DbJourneyLine> JourneyLines { get; set; }

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
                entity.Property(e => e.AccountId).IsRequired();
                entity.Property(e => e.Route).IsRequired();
                entity.Property(e => e.StartLocation).IsRequired();
                entity.Property(e => e.EndLocation).IsRequired();
                entity.Property(e => e.Time).IsRequired();
                entity.Property(e => e.Period).IsRequired();
            });

            modelBuilder.Entity<DbLine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Description);
                entity.Property(e => e.IsDelayed).IsRequired();
            });

            modelBuilder.Entity<DbAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Uid);
                entity.Property(e => e.Token);
            });

            modelBuilder.Entity<DbJourneyLine>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.JourneyId);
                entity.Property(e => e.LineId);
                entity.Property(e => e.Notified);
            });
        }
    }
}