﻿using Common.Contract.Inventory.Interfaces;
using Inventory.Application;
using Inventory.Application.Interfaces.Repositories;
using Inventory.Application.Services;
using Inventory.Infrastructure.DbContexts;
using Inventory.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Inventory.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInventoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationServices(configuration);

            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IStockRepository, StockRepository>();

            return services;
        }
    }
}
