using Inventory.Application.Contracts;
using Inventory.Application.Services;

namespace Inventory.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IInventoryService, InventoryService>();


            var app = builder.Build();


            app.UseHttpsRedirection();
            app.UseRouting();

            app.Run();

        }
    }
}
