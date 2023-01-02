using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public abstract class RepositoryBase<T,TKey> :IRepository<T, TKey> where T : class,IEntity<TKey>
    {
        private readonly DbFactory _dbFactory;
        private  DbSet<T> _dbSet;
        protected DbSet<T> DbSet => _dbSet ?? (_dbSet = _dbFactory.DbContext.Set<T>());

        public RepositoryBase(DbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task Add(T entity)
        {
            DbSet.Add(entity);
        }

        public async Task Delete(TKey id)
        {
            var entity =await DbSet.FindAsync(id);
            if(entity!=null)
            DbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await DbSet.AsNoTracking<T>().ToListAsync();
        }

        public async ValueTask<T> GetById(TKey id)
        {
            return await DbSet.FindAsync(id);
        }

        public Task Update(T entity)
        {
            DbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> FindByCondition(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).FirstOrDefaultAsync();
        }
    }
}