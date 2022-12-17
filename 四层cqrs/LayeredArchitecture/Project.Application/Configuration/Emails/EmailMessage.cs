using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.Emails
{
    public class EmailMessage
    {
        public string From { get; }

        public string To { get; }

        public string Content { get; }

        public EmailMessage(
            string from,
            string to,
            string content)
        {
            this.From = from;
            this.To = to;
            this.Content = content;
        }
    }
}
