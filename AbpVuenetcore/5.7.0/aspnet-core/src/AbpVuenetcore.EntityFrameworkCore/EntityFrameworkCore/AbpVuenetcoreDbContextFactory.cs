using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using AbpVuenetcore.Configuration;
using AbpVuenetcore.Web;

namespace AbpVuenetcore.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AbpVuenetcoreDbContextFactory : IDesignTimeDbContextFactory<AbpVuenetcoreDbContext>
    {
        public AbpVuenetcoreDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AbpVuenetcoreDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AbpVuenetcoreDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AbpVuenetcoreConsts.ConnectionStringName));

            return new AbpVuenetcoreDbContext(builder.Options);
        }
    }
}
