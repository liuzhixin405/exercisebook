using Microsoft.EntityFrameworkCore;

namespace AspNetCoreAOP
{
    public class CommonDbContext:DbContext
    {
        public CommonDbContext(DbContextOptions<CommonDbContext> options):base(options)
        {

        }
    }
}
