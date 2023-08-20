using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IServiceEF;
using EntityEF.Models;
using EntityEF.Extensions;

namespace IServiceEF.DefaultImplement
{
    public class DefaultEfCoreRepository<Context, T> : IBaseRepository<T> where T : class,IEntity where Context : DbContext
    {
        private readonly Context _context;
        public DefaultEfCoreRepository(Context context)
        {
            _context = context;
        }
        public async Task<bool> Add(T entity)
        {
            _context.Add(entity);
           var result =await _context.SaveChangesAsync();
            return result != 0;
        }

         public async Task<bool> Update(T entity)
        {
             //_context.Set<T>().Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> Delete(T entity)
        {
            _context.Remove(entity);
            var result = await _context.SaveChangesAsync();
            return result!=0;
        }

        public async Task<bool> Add(List<T> entity)
        {
            _context.Add(entity);
            var result = await _context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> Update(List<T> entity)
        {
            //_context.Set<T>().Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();
            return result != 0;
        }

        public async Task<bool> Delete(List<T> entity)
        {
            _context.Remove(entity);
            var result = await _context.SaveChangesAsync();
            return result != 0;
        }
        public async Task<T> GetEntity(Expression<Func<T, bool>> expression)
        {
          return await  _context.Set<T>().Where(expression).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }
        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<PageResult<T>> GetPageResult<Req>(PageInput<Req> pagInput) where Req : new()
        {
            return await _context.Set<T>().AsQueryable<T>().GetPageResultAsync(pagInput);
        }

    }
}
