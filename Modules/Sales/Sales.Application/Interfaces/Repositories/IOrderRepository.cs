using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<bool> AddOrderAsync(Domain.Order order);
    }
}
