using Common.Contract.Inventory.Interfaces;
using FluentValidation;
using Inventory.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Inventory.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            FluentValidation.ValidatorOptions.Global.DisplayNameResolver = (type, memberInfo, expression) =>
            {
                if (memberInfo != null)
                {
                    var displayAttr = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                             .FirstOrDefault() as DisplayAttribute;
                    if (displayAttr != null)
                    {
                        return displayAttr.GetName();
                    }

                    var displayNameAttr = memberInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                         .FirstOrDefault() as DisplayNameAttribute;

                    if (displayNameAttr != null)
                    {
                        return displayNameAttr.DisplayName;
                    }
                }

                return memberInfo?.Name;
            };

            return services;
        }
    }
}
