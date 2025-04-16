using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Contracts
{
    public interface IInventoryService
    {
        Task<bool> ProductExists(string sku);
    }
}
