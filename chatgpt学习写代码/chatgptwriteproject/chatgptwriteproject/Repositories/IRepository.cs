using chatgptwriteproject.Context;

namespace chatgptwriteproject.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll();
        ValueTask<TEntity> GetById(int id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        IUnitOfWork UnitOfWork { get; }
    }
}
