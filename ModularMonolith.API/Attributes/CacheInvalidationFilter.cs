using Framework.Cache.Interface;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace ModularMonolith.API.Attributes
{
    public class CacheInvalidationFilter : IAsyncActionFilter
    {
        private readonly ICacheService _cacheService;

        public CacheInvalidationFilter(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();

            var method = context.ActionDescriptor as ControllerActionDescriptor;
            if (method == null) return;

            var invalidateAttributes = method.MethodInfo
                .GetCustomAttributes<CacheInvalidateAttribute>();

            foreach (var attr in invalidateAttributes)
            {
                await _cacheService.RemoveByPrefixAsync(attr.Prefix);
            }
        }
    }
}
