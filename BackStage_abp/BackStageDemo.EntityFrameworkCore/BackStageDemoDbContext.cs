using BackStageDemo.Domain.UserInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace BackStageDemo.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class BackStageDemoDbContext : AbpDbContext<BackStageDemoDbContext>
    {
        public DbSet<Users> Users { get; set; }
        public BackStageDemoDbContext(DbContextOptions<BackStageDemoDbContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
