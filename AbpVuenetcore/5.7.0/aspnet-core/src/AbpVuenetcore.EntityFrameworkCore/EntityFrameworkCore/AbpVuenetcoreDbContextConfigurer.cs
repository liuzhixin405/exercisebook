using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AbpVuenetcore.EntityFrameworkCore
{
    public static class AbpVuenetcoreDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AbpVuenetcoreDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AbpVuenetcoreDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
