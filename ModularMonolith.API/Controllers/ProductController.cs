using Inventory.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Services;

namespace ModularMonolith.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly SalesService _salesService;

        public ProductController(IInventoryService inventoryService, SalesService salesService)
        {
            _inventoryService = inventoryService;
            _salesService = salesService;
        }

        [HttpGet("check/{sku}")]
        public async Task<IActionResult> CheckProductAvailability(string sku)
        {
            // بررسی موجود بودن محصول در انبار
            var isProductAvailable = await _inventoryService.ProductExists(sku);

            if (!isProductAvailable)
                return NotFound("Product not found in inventory.");

            // بررسی امکان فروش محصول
            var canSell = await _salesService.CheckProductAvailability(sku);

            if (canSell)
                return Ok("Product is available for sale.");
            else
                return BadRequest("Product cannot be sold.");
        }
    }
}
