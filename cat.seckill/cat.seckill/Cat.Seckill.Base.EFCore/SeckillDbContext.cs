using Cat.Seckill.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat.Seckill.Base.EFCore
{
    public class SeckillDbContext:DbContext
    {
        public SeckillDbContext(DbContextOptions<SeckillDbContext> options):base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<SeckillGoods> SeckillGoods { get; set; }
        public DbSet<OrderInfo> OrderInfos { get; set; }
    }
}