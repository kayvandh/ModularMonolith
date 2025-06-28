using Framework.ApiResponse;
using Framework.Cache;
using Framework.Cache.Interface;
using Identity.Application.Configuration;
using Identity.Infrastructure;
using Identity.Infrastructure.DbContexts;
using Inventory.Infrastructure;
using Inventory.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ModularMonolith.API.Attributes;
using ModularMonolith.API.Middlewares;
using ModularMonolith.API.Settings;
using ModularMonolith.Infrastructure;
using Sales.Infrastructure;
using Sales.Infrastructure.DbContexts;
using Sales.Infrastructure.Jobs;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();


builder.Services.AddSalesServices(builder.Configuration)
    .AddInventoryServices(builder.Configuration)
    .AddIdentityServices(builder.Configuration);

// Just for Migration
builder.Services.AddScoped<IEnumerable<DbContext>>(provider => new DbContext[]
{
    provider.GetRequiredService<SalesDbContext>(),
    provider.GetRequiredService<InventoryDbContext>(),
    provider.GetRequiredService<IdentityDbContext>(),
});

builder.Services.AddDbContext<SharedDbContext>((serviceProvider, options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 

builder.Services.Configure<TokenConfiguration>(builder.Configuration.GetSection("TokenConfiguration"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    builder.Configuration.Bind("TokenConfiguration", options);
});
builder.Services.AddScoped<CacheInvalidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<CacheInvalidationFilter>();
});

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Modular Monolith API",
        Version = "v1",
        Description = "API for Modular Monolith architecture with Identity, Sales, Inventory, etc."
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Secured API, Insert JWT Token Here. Sample: Bearer eyJhbGciOiJIUzI1NiIsInR...",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme,
            Array.Empty<string>()
        }
    });

});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        return context.ToApiResponse();
    };
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));

builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"))
);

builder.Services.AddScoped<ICacheService>(provider =>
{
    var settings = provider.GetRequiredService<IOptions<CacheSettings>>().Value;
    return settings.Provider switch
    {
        CacheProvider.Redis => new RedisCacheService(provider.GetRequiredService<IDistributedCache>(), provider.GetRequiredService<IConnectionMultiplexer>()),
        _ => new MemoryCacheService(provider.GetRequiredService<IMemoryCache>())
    };
});


//builder.Services.AddHealthChecks()
//    .AddSqlServer(
//        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
//        name: "ApplicationDb",
//        healthQuery: "SELECT 1;",
//        failureStatus: HealthStatus.Unhealthy,
//        tags: new[] { "db", "sql", "app" }
//    );
//builder.Services.AddHealthChecksUI(opt =>
//{
//    opt.SetEvaluationTimeInSeconds(10);
//    opt.MaximumHistoryEntriesPerEndpoint(60);
//    opt.SetApiMaxActiveRequests(1);
//}).AddInMemoryStorage();




var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    await QuartzStartup.ScheduleAllJobsAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapHealthChecks("/health", new HealthCheckOptions
//{
//    Predicate = _ => true,
//    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//});

//app.MapHealthChecksUI();

app.MapControllers();

app.UseHttpsRedirection();
app.UseRouting();

app.UseGeneralExceptionHandling();
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();


app.Run();
