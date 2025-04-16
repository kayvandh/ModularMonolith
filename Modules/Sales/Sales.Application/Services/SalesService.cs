using Inventory.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Services
{
    public class SalesService
    {
        private readonly IInventoryService _inventoryService;

        public SalesService(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<bool> CheckProductAvailability(string sku)
        {
            return await _inventoryService.ProductExists(sku);
        }
    }
}
