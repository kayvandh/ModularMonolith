using Framework.Cache.Interface;
using Microsoft.Extensions.Options;
using ModularMonolith.API.Settings;
using System.Security.Cryptography;
using System.Text;

namespace ModularMonolith.API.Middlewares
{
    //public class ResponseCacheMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly ICacheService _cacheService;

    //    public ResponseCacheMiddleware(RequestDelegate next, ICacheService cacheService)
    //    {
    //        _next = next;
    //        _cacheService = cacheService;
    //    }

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        if (!HttpMethods.IsGet(context.Request.Method))
    //        {
    //            await _next(context);
    //            return;
    //        }

    //        var cacheKey = GenerateCacheKeyFromRequest(context.Request);
    //        var (found, cachedResponse) = await _cacheService.TryGetValueAsync<string>(cacheKey);

    //        if (found)
    //        {
    //            context.Response.ContentType = "application/json";
    //            await context.Response.WriteAsync(cachedResponse!);
    //            return;
    //        }

    //        // Capture the response
    //        var originalBodyStream = context.Response.Body;
    //        using var memoryStream = new MemoryStream();
    //        context.Response.Body = memoryStream;

    //        await _next(context); // Continue down the pipeline

    //        memoryStream.Seek(0, SeekOrigin.Begin);
    //        var responseBody = new StreamReader(memoryStream).ReadToEnd();

    //        // Reset stream and write to original
    //        memoryStream.Seek(0, SeekOrigin.Begin);
    //        await memoryStream.CopyToAsync(originalBodyStream);
    //        context.Response.Body = originalBodyStream;

    //        // Cache response
    //        await _cacheService.SetAsync(cacheKey, responseBody, TimeSpan.FromSeconds(60));
    //    }

    //    private string GenerateCacheKeyFromRequest(HttpRequest request)
    //    {
    //        var key = $"{request.Path}{request.QueryString}";
    //        return $"response:{key.ToLower()}";
    //    }
    //}

    public class ResponseCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICacheService _cacheService;
        private readonly CacheSettings _settings;

        public ResponseCacheMiddleware(RequestDelegate next, ICacheService cacheService, IOptions<CacheSettings> settings)
        {
            _next = next;
            _cacheService = cacheService;
            _settings = settings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!HttpMethods.IsGet(context.Request.Method))
            {
                await _next(context);
                return;
            }

            var cacheKey = GenerateCacheKey(context.Request);
            var (found, cached) = await _cacheService.TryGetValueAsync<CachedResponse>(cacheKey);

            if (found && cached != null)
            {
                var clientETag = context.Request.Headers["If-None-Match"].FirstOrDefault();

                if (clientETag == cached.ETag)
                {
                    context.Response.StatusCode = StatusCodes.Status304NotModified;
                    context.Response.Headers["ETag"] = cached.ETag!;
                    context.Response.Headers["Cache-Control"] = $"public,max-age={cached.MaxAge}";
                    return;
                }

                context.Response.ContentType = "application/json";
                context.Response.Headers["ETag"] = cached.ETag!;
                context.Response.Headers["Cache-Control"] = $"public,max-age={cached.MaxAge}";
                await context.Response.WriteAsync(cached.Body);
                return;
            }

            // Capture the response from downstream
            var originalBody = context.Response.Body;
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await _next(context);

            memStream.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(memStream).ReadToEndAsync();

            // Compute ETag (using a hash of the response body)
            var eTag = GenerateETag(body);

            // Cache the response
            var responseToCache = new CachedResponse
            {
                Body = body,
                ETag = eTag,
                MaxAge = _settings.ResponseCacheDurationSeconds
            };
            await _cacheService.SetAsync(cacheKey, responseToCache, TimeSpan.FromSeconds(_settings.ResponseCacheDurationSeconds));

            // Set headers and return response
            context.Response.Headers["ETag"] = eTag;
            context.Response.Headers["Cache-Control"] = $"public,max-age={_settings.ResponseCacheDurationSeconds}";

            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }

        private static string GenerateETag(string content)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return $"\"{Convert.ToBase64String(hash)}\"";
        }

        private static string GenerateCacheKey(HttpRequest request)
        {
            return $"response:{request.Path}{request.QueryString}".ToLowerInvariant();
        }

        private class CachedResponse
        {
            public string Body { get; set; } = default!;
            public string? ETag { get; set; }
            public int MaxAge { get; set; }
        }
    }


    public static class ResponseCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseCache(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ResponseCacheMiddleware>();
        }
    }
}
