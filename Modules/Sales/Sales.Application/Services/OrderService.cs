using Inventory.Application.Contracts.Interfaces;
using Sales.Application.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IStockService _stockService;

        public OrderService(IStockService stockService)
        {
            _stockService = stockService;
        }

        public string PlaceOrder(string productCode, int quantity)
        {
            if (_stockService.HasStock(productCode, quantity))
                return $"Order placed for {quantity} of {productCode}";
            else
                return $"Not enough stock for {productCode}";
        }
    }
}
