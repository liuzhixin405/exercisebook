using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using Merp.Infrastructure;
using Merp.Accountancy.CommandStack.Sagas;
using Merp.Accountancy.QueryStack.Model;
using Merp.Infrastructure.Impl;
using Merp.Accountancy.CommandStack.Services;
using Merp.Accountancy.QueryStack;
using Merp.Accountancy.QueryStack.Denormalizers;
using Merp.Registry.CommandStack.Sagas;
using Merp.Registry.QueryStack.Denormalizers;
using Merp.Web.UI.Areas.Registry.WorkerServices;
using Merp.Web.UI.Areas.Accountancy.WorkerServices;
using Merp.Accountancy.CommandStack.Handlers;


namespace Merp.Web.UI
{
    public static class Bootstrapper
    {

        public static void Initialise()
        {   
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));            
            RegisterTypes(container);
            
            var bus = container.Resolve<IBus>();
            ConfigureAccountancyBoundedContext(container,bus);
            ConfigureRegistryBoundedContext(container, bus);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        { 
            container.RegisterType<IBus, InMemoryBus>(new InjectionConstructor(container, typeof(IEventStore)));
            container.RegisterType<IEventStore, Merp.Infrastructure.RavenDB.RavenDbEventStore>();
            container.RegisterType<IRepository, Merp.Infrastructure.RavenDB.RavenDbRepository>();
        }

        private static void ConfigureAccountancyBoundedContext(IUnityContainer container, IBus bus)
        {
            //Denormalizers
            bus.RegisterHandler<FixedPriceJobOrderDenormalizer>();
            bus.RegisterHandler<IncomingInvoiceDenormalizer>();
            bus.RegisterHandler<InvoiceDenormalizer>();
            bus.RegisterHandler<OutgoingInvoiceDenormalizer>();
            bus.RegisterHandler<TimeAndMaterialJobOrderDenormalizer>();

            //Handlers
            bus.RegisterHandler<JobOrderHandler>();

            //Sagas
            bus.RegisterSaga<FixedPriceJobOrderSaga>();
            bus.RegisterSaga<IncomingInvoiceSaga>();
            bus.RegisterSaga<OutgoingInvoiceSaga>();
            bus.RegisterSaga<TimeAndMaterialJobOrderSaga>();  

            //Services
            container.RegisterType<IJobOrderNumberGenerator, JobOrderNumberGenerator>();
            container.RegisterType<IOutgoingInvoiceNumberGenerator, OutgoingInvoiceNumberGenerator>();

            //Types
            container.RegisterType<Merp.Accountancy.QueryStack.IDatabase, Merp.Accountancy.QueryStack.Database>();

            //Worker Services
            container.RegisterType<InvoiceControllerWorkerServices, InvoiceControllerWorkerServices>();
            container.RegisterType<JobOrderControllerWorkerServices, JobOrderControllerWorkerServices>();
        }

        private static void ConfigureRegistryBoundedContext(IUnityContainer container, IBus bus)
        {
            //Denormalizers
            bus.RegisterHandler<PersonDenormalizer>();
            bus.RegisterHandler<CompanyDenormalizer>();

            //Handlers

            //Sagas
            bus.RegisterSaga<CompanySaga>();
            bus.RegisterSaga<PersonSaga>();

            //Types
            container.RegisterType<Merp.Registry.QueryStack.IDatabase, Merp.Registry.QueryStack.Database>();

            //Worker Services
            container.RegisterType<PersonControllerWorkerServices, PersonControllerWorkerServices>();
        }
    }
}