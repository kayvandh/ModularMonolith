using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ModularMonolith.Infrastructure
{
    public class SharedDbContextFactory : IDesignTimeDbContextFactory<SharedDbContext>
    {
        public SharedDbContext CreateDbContext(string[] args)
        {
            // Create a ServiceCollection
            var serviceCollection = new ServiceCollection();

            // Build IConfiguration (to load appsettings.json)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Make sure it's the right path for your appsettings.json
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // Register the necessary services, including DbContext
            serviceCollection.AddDbContext<SharedDbContext>(options =>
                options.UseSqlServer(connectionString));  // Set your connection string

            // Build the IServiceProvider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Now get the DbContextOptions from the serviceProvider
            var options = serviceProvider.GetRequiredService<DbContextOptions<SharedDbContext>>();

            // Pass both the DbContextOptions and IServiceProvider to the DbContext constructor
            return new SharedDbContext(options, serviceProvider);
        }
    }
}
