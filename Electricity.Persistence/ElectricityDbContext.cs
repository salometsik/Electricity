using Electricity.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Electricity.Persistence
{
    public class ElectricityDbContext : DbContext
    {
        public ElectricityDbContext(DbContextOptions<ElectricityDbContext> options) : base(options) { }
        public DbSet<ElectricityUsage> ElectricityUsages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
    public class ElectricityDbContextFactory : IDesignTimeDbContextFactory<ElectricityDbContext>
    {
        public ElectricityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ElectricityDbContext>();
            var connectionString = "User Id=postgres;Password=postgres;Host=electricity.database;Database=electricitydb;Port=5432;";
            // Prod
            //var connectionString = "User Id=postgres;Password=postgres;Host=electricity.database;Database=electricitydb;Port=5432;";
            var migrationsAssembly = "Electricity.Persistence";
            optionsBuilder.UseNpgsql(connectionString, a => a.MigrationsAssembly(migrationsAssembly));

            return new ElectricityDbContext(optionsBuilder.Options);
        }
    }
}