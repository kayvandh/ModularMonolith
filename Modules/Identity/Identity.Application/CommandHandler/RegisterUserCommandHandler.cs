using FluentResults;
using Identity.Application.Command;
using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace Identity.Application.CommandHandler
{
    public class RegisterUserCommandHandler
         (UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtTokenService jwtTokenService)
        : IRequestHandler<RegisterUserCommand, FluentResults.Result<Dtos.User>>
    {
        public async Task<Result<User>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.UserName,
                FullName = request.FullName,
                Address = request.Address,
                Email = request.UserName
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                var token = jwtTokenService.GenerateJwtToken(user);
                return Result.Ok(new User
                {
                    Succeeded = true,
                    Token = token
                });
            }

            return Result.Fail("Register Failed")
                .WithErrors(result.Errors.Select(e => new Framework.FluentResultsAddOn.FluentError(e.Code, e.Description)).ToList());

        }
    }
}
