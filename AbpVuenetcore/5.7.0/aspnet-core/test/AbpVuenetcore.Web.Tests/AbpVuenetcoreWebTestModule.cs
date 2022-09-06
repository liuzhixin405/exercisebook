using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpVuenetcore.EntityFrameworkCore;
using AbpVuenetcore.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace AbpVuenetcore.Web.Tests
{
    [DependsOn(
        typeof(AbpVuenetcoreWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class AbpVuenetcoreWebTestModule : AbpModule
    {
        public AbpVuenetcoreWebTestModule(AbpVuenetcoreEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpVuenetcoreWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(AbpVuenetcoreWebMvcModule).Assembly);
        }
    }
}