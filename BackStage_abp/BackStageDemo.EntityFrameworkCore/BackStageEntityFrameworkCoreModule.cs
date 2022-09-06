using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace BackStageDemo.EntityFrameworkCore
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class BackStageEntityFrameworkCoreModule:AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<BackStageDemoDbContext>(opt=> {
                opt.AddDefaultRepositories(true);
            });
            Configure<AbpDbContextOptions>(opt => opt.UseSqlServer());
        }
    }
}
