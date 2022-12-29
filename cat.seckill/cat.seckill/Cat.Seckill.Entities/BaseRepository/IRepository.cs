using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Entities.BaseRepository
{
    public interface IRepository<T>
    {
        Task<IQueryable<T>> FindAll();
        Task<IQueryable<T>> FindByCondition(Expression<Func<T, bool>> expression);
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<T> GetById(int id);
    }
}
