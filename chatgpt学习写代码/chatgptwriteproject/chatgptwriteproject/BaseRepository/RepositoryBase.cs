using chatgptwriteproject.Context;
using chatgptwriteproject.DbFactories;
using chatgptwriteproject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace chatgptwriteproject.BaseRepository
{
    public abstract class RepositoryBase<Ctx,TEntity> : IRepository<TEntity> where TEntity : class where Ctx:DbContext
    {
        private readonly Ctx _dbContext;

        public RepositoryBase(DbFactory<Ctx> dbContext)
        {
            _dbContext = dbContext.Context;
        }

        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public void Add(List<TEntity> entity)
        {
            _dbContext.Set<List<TEntity>>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public void Delete(List<TEntity> entity)
        {
            _dbContext.Set<List<TEntity>>().Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetList()
        {
            var result = await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
            return result;
        }
        public IQueryable<TEntity> GetQuerable()
        {
            return _dbContext.Set<TEntity>().AsQueryable();
        }

        public void Update(TEntity entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.Set<TEntity>().Update(entity);
        }

        public void Update(List<TEntity> entity)
        {
            foreach (var item in entity)
            {
                _dbContext.Entry(item).State = EntityState.Modified;
                _dbContext.Set<TEntity>().Update(item);
            }
        }

        public abstract IUnitOfWork GetUnitOfWork();
    }
}
