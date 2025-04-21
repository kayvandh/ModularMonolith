using Microsoft.EntityFrameworkCore;
using Sales.Domain;

namespace Sales.Infrastructure.DbContexts
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<ScheduledJob> ScheduledJobs { get; set; }
    }
}
