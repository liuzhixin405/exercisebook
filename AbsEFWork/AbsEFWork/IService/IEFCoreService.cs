using AbsEFWork.Models;
using System.Linq.Expressions;

namespace AbsEFWork.IService
{
    public interface IEFCoreService
    {
        Task<bool> Add<T>(T entity);
        Task<bool> Delete<T>(T entity);
        Task<bool> Update<T>(T entity);
        Task<IReadOnlyCollection<T>> GetList<T>(Expression<Func<T,bool>> expression);
        Task<PageResult<T>> GetPageResult<T,Req>(PageInput<Req> pagInput) where Req:new();
        Task<T> GetEntity<T>(Expression<Func<T, bool>> expression);
    }
}
