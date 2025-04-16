using Inventory.Application.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Services
{
    public class StockService : IStockService
    {
        public bool HasStock(string productCode, int quantity)
        {
            return productCode == "P1" && quantity <= 5;
        }
    }
}
