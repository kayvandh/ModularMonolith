using Identity.Application.Contracts.Models;
using Identity.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ModularMonolith.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserModel model)
        {
            var result = await _userService.RegisterUserAsync(model);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { token = result.Token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _userService.SignInUserAsync(model);

            if (!result.Succeeded)
                return Unauthorized(new { errors = result.Errors });

            return Ok(new { token = result.Token });
        }
    }
}
