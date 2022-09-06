using Crawler.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerConsole.DiService
{
    public class ServiceManager
    {
        public static bool TryRegisterService(params Action<IServiceCollection>[] actions)
        {
            try
            {
                CustomApplicationService.RegisterServices(actions);
                CustomApplicationService.BuildServices();
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

    }
}
