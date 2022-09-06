using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpVuenetcore.Configuration;
using AbpVuenetcore.EntityFrameworkCore;
using AbpVuenetcore.Migrator.DependencyInjection;

namespace AbpVuenetcore.Migrator
{
    [DependsOn(typeof(AbpVuenetcoreEntityFrameworkModule))]
    public class AbpVuenetcoreMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AbpVuenetcoreMigratorModule(AbpVuenetcoreEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(AbpVuenetcoreMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AbpVuenetcoreConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpVuenetcoreMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
