using Sales.Application.Interfaces.Repositories;
using Sales.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Persistance
{
    public class OrderRepository : IOrderRepository
    {
        public Task<bool> AddOrderAsync(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
