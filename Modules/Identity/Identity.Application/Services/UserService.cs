using Identity.Application.Contracts;
using Identity.Application.Contracts.Models;
using Identity.Application.Interfaces;
using Identity.Domain;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public UserService(UserManager<ApplicationUser> userManager,
                           SignInManager<ApplicationUser> signInManager,
                           IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<UserResult> RegisterUserAsync(RegisterUserModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                FullName = model.FullName,
                Address = model.Address,
                Email = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var token = _jwtTokenService.GenerateJwtToken(user);
                return new UserResult
                {
                    Succeeded = true,
                    Token = token
                };
            }

            return new UserResult { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };
        }

        public async Task<UserResult> SignInUserAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (signInResult.Succeeded)
                {
                    var token = _jwtTokenService.GenerateJwtToken(user);
                    return new UserResult
                    {
                        Succeeded = true,
                        Token = token
                    };
                }
            }

            return new UserResult { Succeeded = false, Errors = new List<string> { "Invalid login attempt." } };
        }
    }
}
