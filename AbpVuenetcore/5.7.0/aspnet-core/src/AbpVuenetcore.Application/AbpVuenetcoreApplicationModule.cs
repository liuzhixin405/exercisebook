using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AbpVuenetcore.Authorization;

namespace AbpVuenetcore
{
    [DependsOn(
        typeof(AbpVuenetcoreCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AbpVuenetcoreApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AbpVuenetcoreAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AbpVuenetcoreApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
