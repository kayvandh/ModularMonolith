using Sales.Application.Interfaces.Services;

namespace Sales.Application.UseCases
{
    public class SendSmsUseCase
    {
        private readonly ISmsSenderService _smsSenderService;

        public SendSmsUseCase(ISmsSenderService smsSenderService)
        {
            _smsSenderService = smsSenderService;
        }

        public Task ExecuteAsync(string phone)
        {
            return _smsSenderService.SendAsync(phone);
        }
    }
}
