using BackStageDemo.EntityFrameworkCore;
using System;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace BackStageDemo.Application
{
    [DependsOn(
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(BackStageEntityFrameworkCoreModule)
        )]
    public class BackStageDemoApplicationModule:AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(opt => {
                opt.AddProfile<BackStageDemoApplicationAutoMapperProfile>();
            });
        }
    }
}
