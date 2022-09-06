using Lzx.IOCDI.Framework.CustomContainer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lzx.IOCDI.Framework.IOCReplace
{
    /*  
    public interface IServiceProviderFactory<TContainerBuilder>
    {
        TContainerBuilder CreateBuilder(IServiceCollection services);      
        IServiceProvider CreateServiceProvider(TContainerBuilder containerBuilder);
    }
     */
    public class SelfContainerFactory : IServiceProviderFactory<SelfContainerBuilder>
    {
        public SelfContainerBuilder CreateBuilder(IServiceCollection services)
        {
            return new SelfContainerBuilder(services);
        }

        public IServiceProvider CreateServiceProvider(SelfContainerBuilder containerBuilder)
        {
            return containerBuilder.GetServiceProvider();
        }
    }

    public class SelfContainerBuilder
    {
        private static ISelfContainer _container = new SelfContainer();

        public SelfContainerBuilder(IServiceCollection services)
        {
            ServiceCollectionToElevenContainer(services);
        }

        private void ServiceCollectionToElevenContainer(IServiceCollection services)
        {
            foreach (var service in services)
            {
                if(service.ImplementationFactory != null)
                {

                }
                else if (service.ImplementationInstance != null)
                {
                    _container.RegisterType(service.ServiceType, service.GetType());
                }
                else
                {
                    _container.RegisterType(service.ServiceType, service.ImplementationType, TransLifetime(service.Lifetime));
                }
            }
        }

        private LifetimeType TransLifetime(ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    return LifetimeType.Singleton;
                case ServiceLifetime.Scoped:
                    return LifetimeType.Scope;
                case ServiceLifetime.Transient:     
                default:
                    return LifetimeType.Transient;
            }
        }
        public IServiceProvider GetServiceProvider()
        {
            return new SelfServiceProvider(_container);
        }

    }
    public class SelfServiceProvider : IServiceProvider
    {
        private ISelfContainer _container = null;
        private IServiceCollection _iServiceCollection = null;

        public SelfServiceProvider(ISelfContainer container)
        {
            _container = container;
        }

        public SelfServiceProvider(ISelfContainer container, IServiceCollection services)
        {
            _container = container;
            _iServiceCollection = services;
        }
        public object GetService(Type servicetype)
        {
            try
            {
                return _container.Resolve(servicetype);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
