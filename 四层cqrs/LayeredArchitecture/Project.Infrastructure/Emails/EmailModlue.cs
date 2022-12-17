using Autofac;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.Application.Configuration.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Emails
{
    public class EmailModlue:Module
    {
        private readonly IEmailSender sender;
        private readonly EmailsSettings emailsSettings;
        public EmailModlue(IEmailSender sender, EmailsSettings emailsSettings)
        {
            this.sender = sender;
            this.emailsSettings = emailsSettings;
        }
        internal EmailModlue(EmailsSettings emailsSettings)
        {
            this.emailsSettings = emailsSettings;
        }
        protected override void Load(ContainerBuilder builder)
        {
           if(sender != null)
            {
                builder.RegisterInstance(sender);
            }
            else
            {
                builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerLifetimeScope();
            }
            builder.RegisterInstance(emailsSettings);
        }
    }
}
