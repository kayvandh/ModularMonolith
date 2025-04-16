using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Contracts.Interfaces
{
    public interface IOrderService
    {
        string PlaceOrder(string productCode, int quantity);
    }
}
