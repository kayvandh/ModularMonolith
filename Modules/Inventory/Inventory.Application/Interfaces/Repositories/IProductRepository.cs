using System;
using System.Threading.Tasks;

namespace Inventory.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Inventory.Domain.Product> GetByIdAsync(Guid Id);
    }
}
