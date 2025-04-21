
using Sales.Application.Interfaces.Services;

namespace Sales.Application.UseCases
{
    public class SendEmailUseCase
    {
        private readonly IEmailSenderService _emailSenderService;

        public SendEmailUseCase(IEmailSenderService emailSenderService)
        {
            _emailSenderService = emailSenderService;
        }

        public Task ExecuteAsync(string email)
        {
            return _emailSenderService.SendAsync(email);
        }
    }
}
