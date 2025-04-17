using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Interfaces.Repositories
{
    public interface IStockRepository
    {
        Task<bool> DecreaseStockAsync(Guid productId, int quantity);
        Task<int> GetStockAsync(Guid productId);
    }
}
