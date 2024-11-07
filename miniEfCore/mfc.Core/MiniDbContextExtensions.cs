using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public static class MiniDbContextExtensions
    {
        // 为 MiniDbContext 添加查询功能
        public static IEnumerable<TEntity> Where<TEntity>(this MiniDbContext context, Func<TEntity, bool> predicate)
            where TEntity : class
        {
            var dbSet = context.Set<TEntity>();  // 获取 DbSet
            return dbSet.Where(predicate);  // 使用扩展方法进行查询
        }

        // 获取特定实体的第一条记录
        public static TEntity FirstOrDefault<TEntity>(this MiniDbContext context, Func<TEntity, bool> predicate)
            where TEntity : class
        {
            var dbSet = context.Set<TEntity>();  // 获取 DbSet
            return dbSet.FirstOrDefault(predicate);  // 使用扩展方法进行查询
        }

        // 获取实体的数量
        public static int Count<TEntity>(this MiniDbContext context)
            where TEntity : class
        {
            var dbSet = context.Set<TEntity>();  // 获取 DbSet
            return dbSet.Count();
        }

        // 分页查询
        public static IEnumerable<TEntity> SkipTake<TEntity>(this MiniDbContext context, int skip, int take)
            where TEntity : class
        {
            var dbSet = context.Set<TEntity>();  // 获取 DbSet
            return dbSet.SkipTake(skip,take);
        }
    }
}
