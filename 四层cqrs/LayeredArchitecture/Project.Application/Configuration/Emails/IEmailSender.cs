using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.Emails
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailMessage email);
    }
}
