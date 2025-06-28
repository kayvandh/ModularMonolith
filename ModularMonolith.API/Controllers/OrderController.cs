using Framework.ApiResponse;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sales.Application.Commands;

namespace ModularMonolith.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator,ILogger<OrderController> logger)
        {
            _mediator = mediator;
            _logger = logger;   
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderCommand orderCommand)
        {
            _logger.LogInformation("Place Order command: {@OrderCommand}", orderCommand);
            var result = await _mediator.Send(orderCommand);
            return result.ToApiResponse();
            //return new ApiResponse();
        }
    }

}
