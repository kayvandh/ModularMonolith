using Inventory.Application.Contracts;
using System.Threading.Tasks;

namespace Inventory.Application.Services
{
    public class InventoryService : IInventoryService
    {
        public Task<bool> ProductExists(string sku)
        {
            // پیاده‌سازی منطقی که بررسی کنه محصول با این sku وجود داره یا نه
            // برای اینجا فقط یه مثال ساده می‌زنیم.
            return Task.FromResult(sku == "12345");  // فرض می‌کنیم SKU "12345" همیشه موجود است.
        }
    }
}
