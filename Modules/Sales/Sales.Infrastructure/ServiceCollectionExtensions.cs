using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Interfaces.Repositories;
using Sales.Infrastructure.DbContexts;
using System.Reflection;

namespace Sales.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSalesServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddDbContext<SalesDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //services.AddScoped<IOrderRepository, OrderRepository>();

            return services;

        }
    }
}
