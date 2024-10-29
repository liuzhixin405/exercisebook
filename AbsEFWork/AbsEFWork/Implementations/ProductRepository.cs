using BaseEntityFramework.Implementations.Entitys;
using EntityEF.Models;
using IServiceEF;
using IServiceEF.DefaultImplement;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace BaseEntityFramework.Implementations
{
    /// <summary>
    /// 标准基础上封装放这里
    /// </summary>
    public static class ProductRepository
    {
        /// <summary>
        /// 通过扩展方法来实现非标准功能,相比继承下基础的repository框架更容易维护
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Product> GetByIdExtendAsync(this IBaseRepository<Product> repository, int id)
        {
            //todo待实现
            return null;
        }
    }
}
