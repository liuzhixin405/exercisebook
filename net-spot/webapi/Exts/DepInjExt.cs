using webapi.Dals.Impls;
using webapi.Services;
using webapi.Services.Impl;

namespace webapi.Exts
{
    public static class DepInjExt
    {
        public static IServiceCollection AddExtService(this IServiceCollection sc,IConfiguration configuration)
        {
            
            sc.AddSingleton<IBqAccountService,BQAccountServiceImpl>();
            sc.AddTransient<BqAccountDalImpl>(provider =>
            {
                return new BqAccountDalImpl(configuration["ConnectionString"]);
            });
            return sc;
        }
    }
}
