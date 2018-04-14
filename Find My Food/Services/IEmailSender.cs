using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Find_My_Food.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
