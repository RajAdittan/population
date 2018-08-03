
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace population.Model
{
    public class PopulationContext : DbContext
    {

        private static class Holder
        {
            private static readonly PopulationContext populationContext = new PopulationContext();

            public static PopulationContext Instance
            {
                get
                {
                    return populationContext;
                }
            }
        }

        public static PopulationContext SingleInstance { get { return Holder.Instance; } }

        public DbSet<Estimate> Estimates { get; set; }
        public DbSet<Actual> Actuals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=population.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estimate>().HasKey(k => new { k.State, k.Districts });
            modelBuilder.Entity<Actual>().ToTable("Actuals");
            modelBuilder.Entity<Estimate>().ToTable("Estimates");
        }

        public void SaveEstimates()
        {
            this.SaveChanges(true);
            this.Entry(Estimates).Reload();
        }

        public void SaveActuals()
        {
            this.SaveChanges(true);
        }

    }
}