using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAP.MySql.Model
{
    public class CoreDbContext:DbContext
    {
        public virtual DbSet<Order> Order { get; set; } //创建实体类添加Context中
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
        {

        }
       
    }
}
