using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ModularMonolith.Infrastructure
{
    public class SharedDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        // Constructor that accepts DbContextOptions and IServiceProvider
        public SharedDbContext(DbContextOptions<SharedDbContext> options, IServiceProvider serviceProvider)
            : base(options)
        {
            _serviceProvider = serviceProvider;
        }

        // OnModelCreating method that dynamically adds DbSets from all DbContexts
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Get all assemblies loaded in the current domain
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Find all DbContexts in all assemblies
            var dbContexts = assemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(DbContext)) && !t.IsAbstract)
                .ToList();

            foreach (var dbContextType in dbContexts)
            {
                // Resolve the DbContext from the service provider
                var dbContext = _serviceProvider.GetRequiredService(dbContextType);

                // Get all DbSet properties of each DbContext
                var dbSetProperties = dbContext.GetType().GetProperties()
                    .Where(p => p.PropertyType.IsGenericType &&
                                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                    .ToList();

                foreach (var property in dbSetProperties)
                {
                    // Add each DbSet to the model builder
                    modelBuilder.Model.AddEntityType(property.PropertyType.GetGenericArguments()[0]);
                }
            }
        }
    }

}
