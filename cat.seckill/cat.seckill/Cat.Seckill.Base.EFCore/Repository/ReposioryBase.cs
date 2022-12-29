using Cat.Seckill.Entities.BaseRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Repository
{
    public abstract class ReposioryBase<T> : IRepository<T> where T : class
    {
        private readonly SeckillDbContext dbContext;
        public ReposioryBase(SeckillDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task Create(T entity)
        {
           await dbContext.Set<T>().AddAsync(entity);
           await dbContext.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync(); 
        }

        public async Task<IQueryable<T>> FindAll()
        {
            await Task.CompletedTask;
            return dbContext.Set<T>().AsNoTracking();
        }

        public async Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression)
        {
            await Task.CompletedTask;
            return dbContext.Set<T>().AsNoTracking().Where(expression);
        }

        public async Task<T> GetById(int id)
        {
            return await dbContext.Set<T>().FindAsync(id);
        }

        public async Task Update(T entity)
        {
            try
            {
                dbContext.Entry<T>(entity).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
