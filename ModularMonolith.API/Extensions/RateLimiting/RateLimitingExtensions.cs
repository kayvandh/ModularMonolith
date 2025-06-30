using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.RateLimiting;

namespace ModularMonolith.API.Extensions.RateLimiting
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddRateLimitingWithLogging(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Fixed Window - Basic Public Policy
                options.AddFixedWindowLimiter("FixedPolicy", policy =>
                {
                    policy.PermitLimit = 5;
                    policy.Window = TimeSpan.FromSeconds(20);
                    policy.QueueLimit = 0;
                    policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                //  Sliding Window - Per IP
                options.AddSlidingWindowLimiter("SlidingPolicy", policy =>
                {
                    policy.PermitLimit = 10;
                    policy.Window = TimeSpan.FromMinutes(1);
                    policy.SegmentsPerWindow = 3;
                    policy.QueueLimit = 2;
                    policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                //  Token Bucket - Admin API
                options.AddTokenBucketLimiter("AdminPolicy", policy =>
                {
                    policy.TokenLimit = 20;
                    policy.QueueLimit = 5;
                    policy.TokensPerPeriod = 5;
                    policy.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
                    policy.AutoReplenishment = true;
                    policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                //  Concurrency Limiter - Heavy tasks like export
                options.AddConcurrencyLimiter("ExportPolicy", policy =>
                {
                    policy.PermitLimit = 2;
                    policy.QueueLimit = 5;
                    policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.OnRejected = async (context, token) =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("RateLimiter");

                    var path = context.HttpContext.Request.Path;
                    var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                    logger.LogWarning("Rate limit exceeded. Path: {Path}, IP: {IP}", path, ip);

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
                };
            });

            return services;
        }


    }
}
