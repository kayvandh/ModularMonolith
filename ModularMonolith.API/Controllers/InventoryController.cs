using Inventory.Application.Contracts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ModularMonolith.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IStockService _stockService;

        public InventoryController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet("check-stock")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult CheckStock(string productCode, int quantity)
        {
            var result = _stockService.HasStock(productCode, quantity);
            return Ok(new { available = result });
        }
    }
}
