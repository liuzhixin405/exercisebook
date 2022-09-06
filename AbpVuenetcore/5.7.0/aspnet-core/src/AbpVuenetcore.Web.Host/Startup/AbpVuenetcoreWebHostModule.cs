using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpVuenetcore.Configuration;

namespace AbpVuenetcore.Web.Host.Startup
{
    [DependsOn(
       typeof(AbpVuenetcoreWebCoreModule))]
    public class AbpVuenetcoreWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AbpVuenetcoreWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpVuenetcoreWebHostModule).GetAssembly());
        }
    }
}
