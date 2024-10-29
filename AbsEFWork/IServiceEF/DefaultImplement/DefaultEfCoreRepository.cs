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
    public class DefaultEfCoreRepository<T, TContext> : IBaseRepository<T> where T : class,IEntity where TContext : DbContext
    {
        private readonly TContext _context;

        public DefaultEfCoreRepository(TContext context)
        {
            _context =context;
        }
        public async Task<T> Add(T entity)
        {
           var result = await _context.AddAsync(entity);
            return result.Entity;
        }

         public async Task<T> Update(T entity)
        {
            _context.Attach(entity);
            //_context.Set<T>().Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity; // 返回更新后的实体

        }

        public async Task<bool> Delete(object id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Remove(entity);
                return true; // 返回成功标志，提交留给 UnitOfWork
            }
            return false;
        }

        public async Task<bool> Add(List<T> entities)
        {
            await _context.AddRangeAsync(entities);
            return true; // 返回成功标志，提交留给 UnitOfWork
        }

        public async Task<bool> Update(List<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified; // 批量更新
            }
            return true; // 可以根据实际需要返回更详细的结果
        }

        public async Task<bool> Delete(List<object> ids)
        {
            var entities = await Task.WhenAll(ids.Select(async id =>await _context.Set<T>().FindAsync(id)));
            _context.RemoveRange(entities.Where(e => e != null)); // 批量删除
            return true; // 可以根据实际需要返回更详细的结果
        }
        public async Task<T> GetEntity(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(expression); // 返回满足条件的实体
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

       
        public async Task<T> GetById(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}
