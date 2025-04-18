using FluentResults;
using Identity.Application.Command;
using Identity.Application.Dtos;
using Identity.Application.Interfaces;
using Identity.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.CommandHandler
{
    public class LoginCommandHandler
        (UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,IJwtTokenService jwtTokenService) 
        : IRequestHandler<LoginCommand, Result<Dtos.User>>
    {
        public async Task<Result<User>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                var signInResult = await signInManager.PasswordSignInAsync(user, request.Password, false, false);
                if (signInResult.Succeeded)
                {
                    var token = jwtTokenService.GenerateJwtToken(user);

                    return Result.Ok(new User
                    {
                        Succeeded = true,
                        Token = token
                    });
;
                }
            }

            return Result.Fail("Invalid login attempt.");
        }
    }
}
