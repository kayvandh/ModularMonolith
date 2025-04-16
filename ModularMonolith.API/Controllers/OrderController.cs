using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Contracts.Interfaces;

namespace ModularMonolith.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("place")]
        [Authorize(Roles = "Admin")] // فقط ادمین می‌تونه سفارش بده
        public IActionResult PlaceOrder(string productCode, int quantity)
        {
            var message = _orderService.PlaceOrder(productCode, quantity);
            return Ok(new { result = message });
        }
    }

}
