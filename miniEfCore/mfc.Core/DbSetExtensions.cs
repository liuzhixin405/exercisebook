using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public static class DbSetExtensions
    {
        // 为 DbSet 添加 Where 查询方法
        public static IEnumerable<TEntity> Where<TEntity>(this DbSet<TEntity> dbSet, Func<TEntity, bool> predicate)
            where TEntity : class
        {
            return dbSet.ToList().Where(predicate);
        }

        // 为 DbSet 添加 FirstOrDefault 查询方法
        public static TEntity FirstOrDefault<TEntity>(this DbSet<TEntity> dbSet, Func<TEntity, bool> predicate)
            where TEntity : class
        {
            return dbSet.ToList().FirstOrDefault(predicate);
        }

        // 为 DbSet 添加 Count 方法
        public static int Count<TEntity>(this DbSet<TEntity> dbSet)
            where TEntity : class
        {
            return dbSet.ToList().Count;
        }

        // 为 DbSet 添加分页功能
        public static IEnumerable<TEntity> SkipTake<TEntity>(this DbSet<TEntity> dbSet, int skip, int take)
            where TEntity : class
        {
            return dbSet.ToList().Skip(skip).Take(take);
        }

        // 为 DbSet 添加批量更新
        public static void UpdateRange<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities)
            where TEntity : class
        {
            foreach (var entity in entities)
            {
                dbSet.Update(entity); 
            }
        }
    }
}
