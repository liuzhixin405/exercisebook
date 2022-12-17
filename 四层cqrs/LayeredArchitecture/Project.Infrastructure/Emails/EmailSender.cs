using Project.Application.Configuration.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Emails
{
    public class EmailSender:IEmailSender
    {
        public async Task SendEmailAsync(EmailMessage email)
        {
            //:TODO
            return;
        }
    }
}
