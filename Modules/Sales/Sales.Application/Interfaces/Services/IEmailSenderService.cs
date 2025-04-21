using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Application.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task SendAsync(string email);
    }
}
