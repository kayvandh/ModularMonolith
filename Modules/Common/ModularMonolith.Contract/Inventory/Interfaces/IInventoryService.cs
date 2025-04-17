using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModularMonolith.Contract.Inventory.Interfaces
{
    public interface IInventoryService
    {
        Task<FluentResults.Result> DecreaseStockAsync(Guid productId, int quantity);
        Task<FluentResults.Result<int>> GetStockAsync(Guid productId);
    }
}
