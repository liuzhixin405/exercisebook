using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Configuration.Emails;
using Project.Application.Configuration;
using Project.Infrastructure.Caching;
using Project.Infrastructure.Database;
using Project.Infrastructure.Domain;
using Project.Infrastructure.Emails;
using Project.Infrastructure.Logging;
using Project.Infrastructure.Processing.InternalCommands;
using Project.Infrastructure.Processing.Outbox;
using Project.Infrastructure.Processing;
using Project.Infrastructure.Quartz;
using Quartz.Impl;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Infrastructure.SeedWork;
using CommonServiceLocator;
using Autofac.Extras.CommonServiceLocator;

namespace Project.Infrastructure
{
    public class ApplicationStartup
    {
        public static IServiceProvider Initialize(
            IServiceCollection services,
            string connectionString,
            ICacheStore cacheStore,
            IEmailSender emailSender,
            EmailsSettings emailsSettings,
            ILogger logger,
            IExecutionContextAccessor executionContextAccessor,
            bool runQuartz = true)
        {
            if (runQuartz)
            {
                StartQuartz(connectionString, emailsSettings, logger, executionContextAccessor);
            }

            services.AddSingleton(cacheStore);

            var serviceProvider = CreateAutofacServiceProvider(
                services,
                connectionString,
                emailSender,
                emailsSettings,
                logger,
                executionContextAccessor);

            return serviceProvider;
        }

        private static IServiceProvider CreateAutofacServiceProvider(
            IServiceCollection services,
            string connectionString,
            IEmailSender emailSender,
        EmailsSettings emailsSettings,
        ILogger logger,
            IExecutionContextAccessor executionContextAccessor)
        {
            var container = new ContainerBuilder();

            container.Populate(services);

            container.RegisterModule(new LoggingModule(logger));
            container.RegisterModule(new DataAccessModule(connectionString));
            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new DomainModule());

            if (emailSender != null)
            {
                container.RegisterModule(new EmailModule(emailSender, emailsSettings));
            }
            else
            {
                container.RegisterModule(new EmailModule(emailsSettings));
            }

            container.RegisterModule(new ProcessingModule());

            container.RegisterInstance(executionContextAccessor);

            var buildContainer = container.Build();

            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(buildContainer));

            var serviceProvider = new AutofacServiceProvider(buildContainer);

            CompositionRoot.SetContainer(buildContainer);

            return serviceProvider;
        }

        private static void StartQuartz(
            string connectionString,
            EmailsSettings emailsSettings,
            ILogger logger,
            IExecutionContextAccessor executionContextAccessor)
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();

            var container = new ContainerBuilder();

            container.RegisterModule(new LoggingModule(logger));
            container.RegisterModule(new QuartzModule());
            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new DataAccessModule(connectionString));
            container.RegisterModule(new EmailModule(emailsSettings));
            container.RegisterModule(new ProcessingModule());

            container.RegisterInstance(executionContextAccessor);
            container.Register(c =>
            {
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<OrdersContext>();
                dbContextOptionsBuilder.UseSqlServer(connectionString);

                dbContextOptionsBuilder
                    .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

                return new OrdersContext(dbContextOptionsBuilder.Options);
            }).AsSelf().InstancePerLifetimeScope();

            scheduler.JobFactory = new JobFactory(container.Build());

            scheduler.Start().GetAwaiter().GetResult();

            var processOutboxJob = JobBuilder.Create<ProcessOutboxJob>().Build();
            var trigger =
                TriggerBuilder
                    .Create()
                    .StartNow()
                    .WithCronSchedule("0/15 * * ? * *")
                    .Build();

            scheduler.ScheduleJob(processOutboxJob, trigger).GetAwaiter().GetResult();

            var processInternalCommandsJob = JobBuilder.Create<ProcessInternalCommandsJob>().Build();
            var triggerCommandsProcessing =
                TriggerBuilder
                    .Create()
                    .StartNow()
                    .WithCronSchedule("0/15 * * ? * *")
                    .Build();
            scheduler.ScheduleJob(processInternalCommandsJob, triggerCommandsProcessing).GetAwaiter().GetResult();
        }
    }
}
