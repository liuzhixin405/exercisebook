using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using AbpVuenetcore.Authorization.Roles;
using AbpVuenetcore.Authorization.Users;
using AbpVuenetcore.MultiTenancy;

namespace AbpVuenetcore.EntityFrameworkCore
{
    public class AbpVuenetcoreDbContext : AbpZeroDbContext<Tenant, Role, User, AbpVuenetcoreDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public AbpVuenetcoreDbContext(DbContextOptions<AbpVuenetcoreDbContext> options)
            : base(options)
        {
        }
    }
}
