using Framework.Cache.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using ModularMonolith.API.Settings;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ModularMonolith.API.Attributes
{
    public class CachedResponseFilter : IAsyncActionFilter
    {
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _settings;

        public CachedResponseFilter(ICacheService cacheService, IOptions<CacheSettings> settings)
        {
            _cacheService = cacheService;
            _settings = settings.Value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var http = context.HttpContext;
            var request = http.Request;

            var method = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            var attribute = method?.GetCustomAttribute<CachedResponseAttribute>();

            if (attribute == null || request.Method != HttpMethods.Get)
            {
                await next();
                return;
            }

            var cacheKey = $"response:{request.Path}{request.QueryString}";

            var (found, cachedResponseJson) = await _cacheService.TryGetValueAsync<string>(cacheKey);
            if (found && !string.IsNullOrWhiteSpace(cachedResponseJson))
            {
                var cached = JsonSerializer.Deserialize<CachedResponse>(cachedResponseJson);
                if (cached == null)
                {
                    await next();
                    return;
                }

                var clientETag = request.Headers["If-None-Match"].ToString();
                if (!string.IsNullOrWhiteSpace(clientETag) && clientETag == cached.ETag)
                {
                    http.Response.StatusCode = StatusCodes.Status304NotModified;
                    return;
                }

                http.Response.Headers["Cache-Control"] = $"public,max-age={cached.MaxAge}";
                http.Response.Headers["ETag"] = cached.ETag ?? "";

                var result = new ContentResult
                {
                    Content = cached.Body,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };

                context.Result = result;
                return;
            }

            var executedContext = await next();

            if (executedContext.Result is ObjectResult contextResult &&
                contextResult.StatusCode == StatusCodes.Status200OK)
            {
                var body = JsonSerializer.Serialize(contextResult.Value);
                var etag = GenerateETag(body);

                var duration = attribute.DurationSeconds > 0
                    ? attribute.DurationSeconds
                    : _settings.ResponseCacheDurationSeconds;

                var responseToCache = new CachedResponse
                {
                    Body = body,
                    ETag = etag,
                    MaxAge = duration
                };

                var json = JsonSerializer.Serialize(responseToCache);
                await _cacheService.SetAsync(cacheKey, json, TimeSpan.FromSeconds(duration));

                http.Response.Headers["Cache-Control"] = $"public,max-age={duration}";
                http.Response.Headers["ETag"] = etag;
            }
        }

        private static string GenerateETag(string content)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(content));
            return $"\"{Convert.ToBase64String(hash)}\"";
        }

        private class CachedResponse
        {
            public string Body { get; set; } = default!;
            public string? ETag { get; set; }
            public int MaxAge { get; set; }
        }
    }

}
