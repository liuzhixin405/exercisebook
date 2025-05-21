using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices
{
    /// <summary>
    /// service接口_无翻页获取列表数据及统计总记录数接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseService_NoPaging<T>
    {
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Identity of inserted entity</returns>
        Task<int> InsertAsync(T model);
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
    }

    

}
