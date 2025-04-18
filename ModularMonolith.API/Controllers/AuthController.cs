using Framework.ApiResponse;
using Identity.Application.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ModularMonolith.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var restul = await mediator.Send(command);  
            return restul.ToApiResponse();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        { 
            var restul = await mediator.Send(command);  
            return restul.ToApiResponse();
        }
    }
}
