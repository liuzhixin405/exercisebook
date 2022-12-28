using CodeMan.Seckill.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeMan.Seckill.Base.EntityFrameworkCore
{
    public class SeckillDbContext : DbContext
    {
        public SeckillDbContext(DbContextOptions<SeckillDbContext> options)
            : base(options)
{
}
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Goods> Goods { get; set; }
        public DbSet<SeckillGoods> SeckillGoods { get; set; }
        public DbSet<OrderInfo> OrderInfos { get; set; }
    }
}