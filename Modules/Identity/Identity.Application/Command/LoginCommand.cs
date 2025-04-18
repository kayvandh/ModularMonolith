using MediatR;


namespace Identity.Application.Command
{
    public class LoginCommand : IRequest<FluentResults.Result<Dtos.User>>
    {
        public string UserName { get; set; }  // یا ایمیل
        public string Password { get; set; }
    }
}
