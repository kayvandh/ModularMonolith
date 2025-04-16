using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Contracts.Interfaces
{
    public interface IStockService
    {
        bool HasStock(string productCode, int quantity);
    }
}
