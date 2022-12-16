using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public abstract class RepositoryBase<T> where T : class
    {
        private readonly OrderDbContext _;
        public RepositoryBase(OrderDbContext context)
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

        public async Task Delete(int id)
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

        public async ValueTask<T> GetById(int id)
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