
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FXH.Common.DapperService;
using Pandora.Cigfi.IServices; 
using Pandora.Cigfi.Models.Sys;

namespace Pandora.Cigfi.Services.Sys
{
    public interface IAdminService : IBaseService_NoPaging<Sys_AdminModel>
    {
        /// <summary>
        /// 记录用户的登录行为
        /// </summary>
        /// <returns></returns>
        Task<bool> UserloginLogAsync(Sys_AdminModel adminModel, string ip, string msg, AdminLogLevel logLevel);
        /// <summary>
        ///根据手机号获取用户信息
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        Task<Sys_AdminModel> GetUserInfoByMobileAsync(string mobile);
        #region session及cookie的用户信息
        /// <summary>
        /// 设置管理员信息，写入Session 和Cookies
        /// </summary>
        /// <param name="adminName">管理员帐号</param>
        /// <param name="adminPwd">管理员密码，经过加密后的</param>
        /// <param name="adminID">管理员ID</param>
        /// <param name="isSupperAdmin">是否是超级管理员</param>
        /// <param name="adminPower">管理员管理权限</param>
        /// <param name="adminLogID">后台日志ID</param>
        /// <param name="adminSalt">加密盐</param>
        
        //void SetAdminInfo(string adminName, string adminPwd, int adminID, int isSupperAdmin, string adminPower, string adminLogID, string adminSalt);
        /// <summary>
        /// 管理员退出登录，清除信息
        /// </summary>
        void ClearAdminInfo();
        #endregion

        #region 用户信息


        /// <summary>
        /// 判断当前用户是否已登录
        /// </summary>
        /// <returns></returns>
        Task<bool> IsLogin();

        /// <summary>
        /// 获取登录用户的信息
        /// </summary>
        /// <returns>当前管理员实体</returns>
        Task<Sys_AdminModel> GetAdminInfoAsync(string userName);
        #endregion

        /// <summary>
        /// 是否存在相同帐号名的记录
        /// </summary>
        /// <param name="id">不等于当前ID</param>
        /// <param name="userName">用户帐号</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(int id,string userName);


        /// <summary>
        /// 根据用户帐号获取用户信息
        /// </summary>
        /// <param name="userName">帐号</param> 
        /// <returns></returns>
        Task<Sys_AdminViewModel> GetUserByNameAsync(string userName);

        /// <summary>
        /// 检查校验用户的登录行为
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="userPwd">密码</param>
        /// <returns></returns>
        Task<bool> CheckUserAsync(string userName, string userPwd, string ip);
        

        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        Task<IEnumerable<Sys_AdminViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20);
        /// <summary>
        /// 根据过滤条件统计符合条件的记录数
        /// </summary>
        /// <param name="queryHt"></param>
        /// <returns></returns>
        Task<int> CountAsync(Hashtable queryHt);

    }
}
