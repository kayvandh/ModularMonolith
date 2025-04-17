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
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderCommand orderCommand)
        {
            var result = await _mediator.Send(orderCommand);
            return result.ToApiResponse(); 
        }
    }

}
