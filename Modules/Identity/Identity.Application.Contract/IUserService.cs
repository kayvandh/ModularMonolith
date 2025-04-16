using Identity.Application.Contracts.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Contracts
{
    public interface IUserService
    {
        Task<UserResult> RegisterUserAsync(RegisterUserModel model);
        Task<UserResult> SignInUserAsync(LoginModel model);
    }
}
