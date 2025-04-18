using MediatR;

namespace Identity.Application.Command
{
    public class RegisterUserCommand : IRequest<FluentResults.Result<Dtos.User>>
    {
        public string UserName { get; set; }  
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
    }
}
