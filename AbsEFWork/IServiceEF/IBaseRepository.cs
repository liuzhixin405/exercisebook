using EntityEF.Dto;
using EntityEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IServiceEF
{
    public interface IBaseRepository
        <T> where T : IEntity 
    {

        Task<T> Add(T entity);
        Task<bool> Delete(object entity);
        Task<T> Update(T entity);

        Task<bool> Add(List<T> entity);
        Task<bool> Delete(List<object> entity);
        Task<bool> Update(List<T> entity);

        Task<PageResult<T>> GetPageResult<Req>(PageInput<Req> pagInput) where Req : new();
        Task<T> GetEntity(Expression<Func<T, bool>> expression);
       
        Task<T> GetById(object id);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true);


    }
}
