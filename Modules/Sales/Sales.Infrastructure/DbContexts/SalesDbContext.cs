using Microsoft.EntityFrameworkCore;
using Sales.Domain;
using System.Reflection;

namespace Sales.Infrastructure.DbContexts
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<ScheduledJob> ScheduledJobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
