using Framework.Persistance.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Persistance
{
    public static class UnitOfWorkIocExtensions
    {
        public static IServiceCollection AddSecureUnitOfWork(this IServiceCollection services, Action<UnitOfWorkOptions> configureOptions)
        {
            var options = new UnitOfWorkOptions();
            configureOptions(options);

            services.AddScoped<IUnitOfWork>(provider =>
            {
                var dbContext = provider.GetRequiredService<DbContext>();
                return new UnitOfWork(dbContext, options.UserId, options.Roles);
            });

            return services;
        }
    }
}
