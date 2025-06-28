using Inventory.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistance
{
    public class StockRepository : IStockRepository
    {
        public Task<bool> DecreaseStockAsync(Guid productId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetStockAsync(Guid productId)
        {
            throw new NotImplementedException();
        }
    }
}
