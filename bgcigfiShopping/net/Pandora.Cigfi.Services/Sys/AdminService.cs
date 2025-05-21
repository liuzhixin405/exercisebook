 
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FXH.Common.DapperService;
using Pandora.Cigfi.Services.Sys;
using Pandora.Cigfi.Models.Sys;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Common;
using FXH.Web.Extensions.Http;
using Pandora.Cigfi.Models.Consts.Sys;
using FXH.Common.Logger;
using FXH.Common.Data.Security;
using System.Text;

namespace Pandora.Cigfi.IServices.Sys
{
    public class AdminService : BaseRepository<Sys_AdminModel>, IAdminService
    {
        private IAdminLogService _adminLogService;

        public AdminService(IAdminLogService adminLogService, IDapperRepository context) : base(context)
        {
            _adminLogService = adminLogService;

        }

        /// <summary>
        ///根据手机号获取用户信息
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public async Task<Sys_AdminModel> GetUserInfoByMobileAsync(string mobile)
        {
            var sql = $"select * from  {TableNameConst.SYS_ADMIN} where Tel=@Tel";

            using (var connection = DbContext.Connection)
            {
                return await connection.QueryFirstOrDefaultAsync<Sys_AdminModel>(sql,new { Tel = mobile });
            }
        }

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
        /*public void SetAdminInfo(string adminName, string adminPwd, int adminID, int isSupperAdmin, string adminPower, string adminLogID, string adminSalt)
        {
            try
            {
                SessionHelper.WriteSession(AdminKeyConsts.ADMINID, DESHelper.Encode(adminID.ToString(), AdminKeyConsts.DESKEY));

                int cookieTimeOut = 120;
                //写入 session
              
                SessionHelper.WriteSession(AdminKeyConsts.ADMINNAME, DESHelper.Encode(adminName,AdminKeyConsts.DESKEY) );
                SessionHelper.WriteSession(AdminKeyConsts.ADMINPOWER, adminPower);
                SessionHelper.WriteSession(AdminKeyConsts.ISSUPPERADMIN, DESHelper.Encode(isSupperAdmin.ToString(), AdminKeyConsts.DESKEY));
                SessionHelper.WriteSession(AdminKeyConsts.ADMINLOGID, adminLogID);

                //写入cookie 
                CookiesHelper.WriteCookie(AdminKeyConsts.ADMINID, DESHelper.Encode(adminID.ToString(), AdminKeyConsts.DESKEY), cookieTimeOut);
                CookiesHelper.WriteCookie(AdminKeyConsts.ADMINNAME, DESHelper.Encode(adminName, AdminKeyConsts.DESKEY), cookieTimeOut);
                CookiesHelper.WriteCookie(AdminKeyConsts.ISSUPPERADMIN, DESHelper.Encode(isSupperAdmin.ToString(), AdminKeyConsts.DESKEY) , cookieTimeOut);
                //2016-6-01 增加IP加密信息，防止cookies被盗用
                CookiesHelper.WriteCookie(AdminKeyConsts.ADMININFO, CommonUtils.MD5(GetUserInfoValue(  adminName,    adminPwd,   adminSalt)), cookieTimeOut);
                CookiesHelper.WriteCookie(AdminKeyConsts.ADMINLOGID, adminLogID, cookieTimeOut);

            }
            catch (Exception ex)
            {
                LogExtension.LogWarn(string.Format("写session及cookie出错，原因:{0}", ex.Message));
            }

        }*/


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
        public void SetAdminInfoSession(string adminName, string phone, int adminID, int isSupperAdmin, string adminPower, string adminLogID, string adminSalt)
        {
            try
            {
                SessionHelper.WriteSession(AdminKeyConsts.ADMINID, DESHelper.Encode(adminID.ToString(), AdminKeyConsts.DESKEY));

                int cookieTimeOut = 60*16;//16个小时
                //写入 session

                SessionHelper.WriteSession(AdminKeyConsts.ADMINNAME, DESHelper.Encode(adminName, AdminKeyConsts.DESKEY));
                SessionHelper.WriteSession(AdminKeyConsts.ADMINPOWER, adminPower);
                SessionHelper.WriteSession(AdminKeyConsts.ISSUPPERADMIN, DESHelper.Encode(isSupperAdmin.ToString(), AdminKeyConsts.DESKEY));
                SessionHelper.WriteSession(AdminKeyConsts.ADMINLOGID, adminLogID);

                //写入cookie 
                CookiesHelper.WriteCookie(AdminKeyConsts.ADMINID, DESHelper.Encode(adminID.ToString(), AdminKeyConsts.DESKEY), cookieTimeOut);
                CookiesHelper.WriteCookie(AdminKeyConsts.ADMINNAME, DESHelper.Encode(adminName, AdminKeyConsts.DESKEY), cookieTimeOut);
                CookiesHelper.WriteCookie(AdminKeyConsts.ISSUPPERADMIN, DESHelper.Encode(isSupperAdmin.ToString(), AdminKeyConsts.DESKEY), cookieTimeOut);
                //2016-6-01 增加IP加密信息，防止cookies被盗用
                CookiesHelper.WriteCookie(AdminKeyConsts.ADMININFO, CommonUtils.MD5(GetUserInfoValue(adminName ,adminSalt)), cookieTimeOut);
                CookiesHelper.WriteCookie(AdminKeyConsts.ADMINLOGID, adminLogID, cookieTimeOut);
            }
            catch (Exception ex)
            {
                LogExtension.LogWarn(string.Format("写session及cookie出错，原因:{0}", ex.Message));
            }

        }

        /// <summary>
        /// 管理员退出登录，清除信息
        /// </summary>
        public void ClearAdminInfo()
        {
            //写入 session
            SessionHelper.WriteSession(AdminKeyConsts.ADMINID, null);
            SessionHelper.WriteSession(AdminKeyConsts.ADMINNAME, null);
            SessionHelper.WriteSession(AdminKeyConsts.ADMINPOWER, null);
            SessionHelper.WriteSession(AdminKeyConsts.ISSUPPERADMIN, null);
            SessionHelper.WriteSession(AdminKeyConsts.ADMINLOGID, null);

            //写入cookie 
            CookiesHelper.ClearCookies(AdminKeyConsts.ADMINID);
            CookiesHelper.ClearCookies(AdminKeyConsts.ADMINNAME);
            CookiesHelper.ClearCookies(AdminKeyConsts.ADMININFO);
            CookiesHelper.ClearCookies(AdminKeyConsts.ADMINLOGID);
        }

        #endregion


        #region 用户登录信息

        /// <summary>
        /// 判断当前用户是否已登录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsLogin()
        {
            try
            {
                string adminName = SessionHelper.GetSession(AdminKeyConsts.ADMINNAME).ToString();//用户名
                string adminID = SessionHelper.GetSession(AdminKeyConsts.ADMINID).ToString();//ID

                //如果Session失效，则用Cookies判断
                if (string.IsNullOrEmpty(adminName) || string.IsNullOrEmpty(adminID))
                {
                    string cooAdminName = CookiesHelper.GetCookie(AdminKeyConsts.ADMINNAME);//用户名
                    string cooAdminID = CookiesHelper.GetCookie(AdminKeyConsts.ADMINID);//ID
                    string cooLoginInfo = CookiesHelper.GetCookie(AdminKeyConsts.ADMININFO);//信息
                    string cooAdminLogID = CookiesHelper.GetCookie(AdminKeyConsts.ADMINLOGID);//日志GUID

                    if (string.IsNullOrEmpty(cooAdminID) || string.IsNullOrEmpty(cooAdminName) || string.IsNullOrEmpty(cooLoginInfo) || string.IsNullOrEmpty(cooAdminLogID))
                    {
                        return false;//信息不完整
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(cooAdminName))
                        {
                            cooAdminName = DESHelper.Decode(cooAdminName, AdminKeyConsts.DESKEY);
                        }
                        if (!string.IsNullOrEmpty(cooAdminID))
                        {
                            cooAdminID = DESHelper.Decode(cooAdminID, AdminKeyConsts.DESKEY);
                        }
                        //全不为空则判断信息是否正确
                        Sys_AdminModel adminModel = await GetUserByNameAsync(cooAdminName);

                        if (adminModel == null)
                        {
                            return false;//找不到管理员
                        }
                        else
                        {
                            var cusUserInfo = CommonUtils.MD5(GetUserInfoValue(adminModel.UserName,  adminModel.Salt));
                            var isTimeOut = (DateTime.Now - adminModel.LastLoginTime).TotalSeconds >= 6*60*60;
                            if (cusUserInfo == cooLoginInfo && !isTimeOut)
                            {
                                //重新写入Session 和 Cookies
                                SetAdminInfoSession(adminModel.UserName, adminModel.Tel, adminModel.Id, 0, "", cooAdminLogID, adminModel.Salt);
                                return true;
                            }
                            else
                            {
                                ClearAdminInfo();//清除信息
                                return false;//信息错误
                            }
                        }
                    }
                }
                else
                {
                    return true;//Session未失效，正确
                }

            }
            catch(Exception ex)
            {
                LogExtension.LogError("判断用户是否登录出错，原因：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取登录用户的信息
        /// </summary>
        /// <returns>当前管理员实体</returns>
        public async Task<Sys_AdminModel> GetAdminInfoAsync(string userName)
        {
            if (await IsLogin())
            {
                return await GetUserByNameAsync(userName);
            }
            else
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 是否存在相同帐号名的记录
        /// </summary>
        /// <param name="id">不等于当前ID</param>
        /// <param name="userName">用户帐号</param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(int id, string userName)
        {
            string sql = string.Format(@"select  count(*)
                        from {0}    where Id!=@Id and UserName=@UserName ", TableNameConst.SYS_ADMIN);

            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, new { Id = id, UserName = userName }) > 0;
            }
        }
        /// <summary>
        /// 根据用户帐号获取用户信息
        /// </summary>
        /// <param name="userName">帐号</param> 
        /// <returns></returns>
        public async Task<Sys_AdminViewModel> GetUserByNameAsync(string userName)
        {
            try
            {
                string sql = string.Format("select a.*,b.RoleName from  {0} a left join  {1} b on a.RoleId=b.Id where UserName=@UserName  limit 1 ", TableNameConst.SYS_ADMIN, TableNameConst.SYS_ADMINROLES);
                var parameters = new DynamicParameters();
                parameters.Add("UserName", userName);
                using (var connection = DbContext.Connection)
                {
                    return await connection.QueryFirstAsync<Sys_AdminViewModel>(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 检查校验用户的登录行为
        /// </summary>
        /// <param name="userName">帐号</param>
        /// <param name="userPwd">密码</param>
        /// <returns></returns>
        public async Task<bool> CheckUserAsync(string userName, string userPwd, string ip)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPwd))
                return false;

            userPwd = userPwd.Trim();
            userName = CommonUtils.SqlStr(userName.Trim());
            Guid GUID = System.Guid.NewGuid();
            Sys_AdminModel adminModel = await GetUserByNameAsync(userName);
            Sys_AdminLogModel logModel = new Sys_AdminLogModel();

            logModel.GUID = GUID.ToString();
            logModel.LogType = Convert.ToInt16(AdminLogType.LOGIN).ToString();

            logModel.LogIP = ip;
            logModel.LogTime = DateTime.Now;
            logModel.UserName = userName.Trim();
            logModel.LogLevel = Convert.ToInt16(AdminLogLevel.NORMAL);

           

            if (adminModel == null || string.IsNullOrEmpty(adminModel.UserName))
            {
                logModel.LogLevel = Convert.ToInt16(AdminLogLevel.WARN);
                logModel.LogMsg = string.Format("{0}登录失败：用户名错误。", userName);
                await _adminLogService.InsertAsync(logModel);
                return false;
            }
            else if (adminModel.PassWord != CommonUtils.MD5(adminModel.Salt + userPwd.Trim()))
            {
                logModel.LogType = Convert.ToInt16(AdminLogType.LOGINFAIL).ToString();

                logModel.LogLevel = Convert.ToInt16(AdminLogLevel.WARN);
                logModel.LogMsg = string.Format("{0}登录失败：密码错误。", userName);
                await _adminLogService.InsertAsync(logModel);
                return false;
            }
            else
            {
                adminModel.LastLoginTime = DateTime.Now;
                await base.UpdateAsync(adminModel);

                logModel.UId = adminModel.Id;
                logModel.LogMsg = string.Format("{0}登录成功", userName);
                await _adminLogService.InsertAsync(logModel);
                //写入Session 和 Cookies

                SetAdminInfoSession(adminModel.UserName, adminModel.PassWord, adminModel.Id, 0, "", GUID.ToString(), adminModel.Salt);
                return true;
            }

        }

        /// <summary>
        /// 记录用户的登录行为
        /// </summary>
        /// <param name="ip">用户登录ip</param>
        /// <param name="msg">登录结果</param>
        /// <returns></returns>
        public async Task<bool> UserloginLogAsync(Sys_AdminModel adminModel, string ip, string msg, AdminLogLevel logLevel)
        {
            Guid GUID = System.Guid.NewGuid();
            Sys_AdminLogModel logModel = new Sys_AdminLogModel();
            if (adminModel !=null)
            {
                logModel.UserName = adminModel.UserName;
                adminModel.LastLoginTime = DateTime.Now;
                logModel.UId = adminModel.Id;
            }
            logModel.GUID = GUID.ToString();
            logModel.LogType = Convert.ToInt16(AdminLogType.LOGIN).ToString();
            logModel.LogIP = ip;
            logModel.LogTime = DateTime.Now;
            logModel.LogLevel = Convert.ToInt16(logLevel);
            logModel.LogMsg = msg;

            await _adminLogService.InsertAsync(logModel);
            if (logLevel.ToInt()==1)
            {
                await base.UpdateAsync(adminModel);
                //写入Session 和 Cookies
                SetAdminInfoSession(adminModel.UserName, adminModel.Tel, adminModel.Id, 0, "", GUID.ToString(), adminModel.Salt);
            }
            return true;
        }

        #region 分页显示
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        public async Task<IEnumerable<Sys_AdminViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = string.Format(@"select a.Id,UserName,RoleName,RealName,LastLoginTime
                        from {0} a  left join  {1} b
                        on a.RoleId = b.Id  ", TableNameConst.SYS_ADMIN, TableNameConst.SYS_ADMINROLES);
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += string.Format(" order by Id desc  limit {0} ,{1}", (page - 1) * limit, limit);

            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<Sys_AdminViewModel>(sql, parameters);

            }
        }



        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = string.Format(@"select  count(*) 
                         from {0} a  left join  {1} b
                        on a.RoleId = b.Id   ", TableNameConst.SYS_ADMIN, TableNameConst.SYS_ADMINROLES);
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
        }
        private string GetWhere(Hashtable queryHt, DynamicParameters parameters)
        {
            string where = string.Empty;
            if (queryHt.Count > 0)
            {
                if (queryHt.Contains("keyword"))
                {
                    where += "    (UserName LIKE @Keyword OR RealName LIKE @Keyword)";
                    parameters.Add("Keyword", $"%{queryHt["keyword"].ToString()}%");
                }
                if (queryHt.Contains("roleid"))
                {
                    if (where.Length > 0)
                    {
                        where += " and ";
                    }
                    where += "    RoleId=@RoleId ";
                    parameters.Add("RoleId", queryHt["roleid"].ToString());
                }
            }
            if (where.Length > 0)
            {
                where = " where " + where;
            }
            return where;
        }
        #endregion


        private string GetUserInfoValue(string adminName,  string adminSalt)
        {
            return string.Format("{0}_{1}_{2}", adminName , adminSalt, CommonUtils.GetIP());
        }
    }


}
