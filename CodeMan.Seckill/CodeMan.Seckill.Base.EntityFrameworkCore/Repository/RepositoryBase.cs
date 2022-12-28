using System;
using System.Linq;
using System.Linq.Expressions;
using CodeMan.Seckill.Service.Repository;
using Microsoft.EntityFrameworkCore;

namespace CodeMan.Seckill.Base.EntityFrameworkCore.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected SeckillDbContext SeckillDbContext { get; set; }

        protected RepositoryBase(SeckillDbContext repositoryContext)
        {
            SeckillDbContext = repositoryContext;
        }
        public IQueryable<T> FindAll()
        {
            return SeckillDbContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return SeckillDbContext.Set<T>().Where(expression);
        }

        public void Create(T entity)
        {
            SeckillDbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            SeckillDbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            SeckillDbContext.Set<T>().Remove(entity);
        }
    }
}