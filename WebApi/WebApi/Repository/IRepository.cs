using System.Linq.Expressions;

namespace WebApi.Repository
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        ValueTask<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
    }
}
