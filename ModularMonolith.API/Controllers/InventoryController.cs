using Azure.Core;
using Framework.ApiResponse;
using Inventory.Application.Commands;
using Inventory.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ModularMonolith.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetStock(GetStockQuery request)
        {
            var stock = await _mediator.Send(request);
            return stock.ToApiResponse();
        }

        [HttpPost("decrease")]
        public async Task<IActionResult> DecreaseStock([FromBody] DecreaseStockCommand command)
        {
            var result = await _mediator.Send(command);
            return result.ToApiResponse();
        }
    }
}
