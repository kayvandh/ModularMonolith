using Sales.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Infrastructure.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        public Task SendAsync(string email)
        {
            Console.WriteLine($"[EMAIL] to {email}");
            return Task.CompletedTask;
        }
    }
}
