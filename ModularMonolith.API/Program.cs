using Identity.Application.Configuration;
using Identity.Application.Interfaces;
using Identity.Infrastructure;
using Identity.Infrastructure.DbContexts;
using Identity.Infrastructure.Services;
using Inventory.Infrastructure;
using Inventory.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ModularMonolith.Infrastructure;
using Sales.Infrastructure;
using Sales.Infrastructure.DbContexts;

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


builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
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

builder.Services.AddAuthorization();
builder.Services.AddControllers();
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
        Description = "برای دسترسی به APIهای محافظت‌شده، توکن JWT را در این فیلد وارد کنید. \n\nمثال: Bearer eyJhbGciOiJIUzI1NiIsInR...",
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapControllers();

app.UseHttpsRedirection();
app.UseRouting();

app.Run();
