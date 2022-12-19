using System.Linq.Expressions;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface IRepository<TEntity,TKey> where TEntity:IEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAll();
        ValueTask<TEntity> GetById(TKey id);
        Task Add(TEntity entity);
        Task Update(TEntity entity);
        Task Delete(TKey id);
        Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
