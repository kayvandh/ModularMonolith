using Sales.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Infrastructure.Services
{
    public class SmsSenderService : ISmsSenderService
    {
        public Task SendAsync(string phone)
        {
            Console.WriteLine($"[SMS] to {phone}");
            return Task.CompletedTask;
        }
    }
}
