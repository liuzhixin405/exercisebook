using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public abstract class RepositoryBase<T,TKey> :IRepository<T, TKey> where T : class,IEntity<TKey>
    {
        private readonly DbContext _;
        public RepositoryBase(DbContext context)
        {
            _=context;
        }

        public async Task Add(T entity)
        {
            try
            {
                await _.Set<T>().AddAsync(entity);
               var result =  await _.SaveChangesAsync();
            }
            catch
            {

            }
           
        }

        public async Task Delete(TKey id)
        {
            var entity = await _.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _.Set<T>().Remove(entity);
                await _.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _.Set<T>().AsNoTracking<T>().ToListAsync();
        }

        public async ValueTask<T> GetById(TKey id)
        {
            return await _.Set<T>().FindAsync(id);
        }

        public async Task Update(T entity)
        {
            try
            {
                _.Entry<T>(entity).State = EntityState.Modified;
                await _.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _.Set<T>().Where(predicate).ToListAsync();
        }
    }
}