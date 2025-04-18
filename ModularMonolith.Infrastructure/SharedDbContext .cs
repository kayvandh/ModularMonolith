using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ModularMonolith.Infrastructure
{
    public class SharedDbContext:  DbContext
    {
        private readonly IEnumerable<DbContext> _moduleDbContexts;
        public SharedDbContext(DbContextOptions<SharedDbContext> options, IEnumerable<DbContext> moduleDbContexts)
            : base(options)
        {
            _moduleDbContexts = moduleDbContexts;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var dbContext in _moduleDbContexts)
            {               
                modelBuilder.ApplyConfigurationsFromAssembly(dbContext.GetType().Assembly);
            }
        }
    }
}



