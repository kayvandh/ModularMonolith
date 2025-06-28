using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Simpl;
using Sales.Application.Commands;
using Sales.Application.Interfaces.Repositories;
using Sales.Application.Interfaces.Services;
using Sales.Application.Persistance;
using Sales.Application.UseCases;
using Sales.Infrastructure.DbContexts;
using Sales.Infrastructure.Jobs.DynamicJobs;
using Sales.Infrastructure.Jobs.StaticJobs.Setup;
using Sales.Infrastructure.Services;
using System.Reflection;

namespace Sales.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSalesServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrderCommand).Assembly));

            services.AddDbContext<SalesDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            ConfigureQuartz(services, configuration);

            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;

        }

        private static void ConfigureQuartz(IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UsePersistentStore(opt =>
                {
                    opt.UseProperties = true;
                    opt.UseSqlServer(sqlOptions =>
                    {
                        sqlOptions.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                    });
                    opt.UseJsonSerializer();
                });
            });

            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            services.AddScoped<SendEmailUseCase>();
            services.AddScoped<SendSmsUseCase>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<ISmsSenderService, SmsSenderService>();            
            services.AddScoped<SendEmailJob>();
            services.AddScoped<SendSmsJob>();


            //services.ConfigureOptions<LoggingBackGroundJobSetup>();
        }
    }
}
