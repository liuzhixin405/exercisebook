using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Crawler.Service
{
    public class CustomApplicationService
    {
        private static IServiceCollection services;
        private static IServiceProvider serviceProvider;

        public static void RegisterServices(params Action<IServiceCollection>[] actions)
        {
            services = new ServiceCollection();
            foreach (var action in actions)
            {
                action?.Invoke(services);
            }
        }

        public static void BuildServices()
        {
            if (services == null)
            {
                throw new Exception("服务为注册, 请先注册服务");
            }

            serviceProvider = services.BuildServiceProvider();
        }
        public static object GetService(Type type)
        {
           return serviceProvider.GetService(type);
        }

        public static T GetService<T>()
        {
            return serviceProvider.GetService<T>();
        }
    }
}
