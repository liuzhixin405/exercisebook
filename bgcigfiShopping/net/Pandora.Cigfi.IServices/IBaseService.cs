using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices
{    
    public interface IBaseService<T>
    {
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Identity of inserted entity</returns>
        new Task<int> InsertAsync(T model);
        /// <summary>
        /// 更新模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T model);
        /// <summary>
        /// 根据id获取模型
        /// </summary>
        /// <typeparam name="TID"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync<TID>(TID id);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T model);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="list">对象列表</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(List<T> list);
        /// <summary>
        /// 获取Model集合（没有Where条件）
        /// </summary> 
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20);
        /// <summary>
        /// 根据过滤条件统计符合条件的记录数
        /// </summary>
        /// <param name="queryHt"></param>
        /// <returns></returns>
        Task<int> CountAsync(Hashtable queryHt);
    }

    

}
