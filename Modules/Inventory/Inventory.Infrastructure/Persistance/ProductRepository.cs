using Inventory.Application.Interfaces.Repositories;
using Inventory.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Persistance
{
    public class ProductRepository : IProductRepository
    {
        public Task<Product> GetByIdAsync(Guid Id)
        {
            throw new NotImplementedException();
        }
    }
}
