 
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FXH.Common.DapperService;
using Pandora.Cigfi.Services.Sys;
using Pandora.Cigfi.Models.Sys;
using Pandora.Cigfi.Models.Consts;
using System.Text;

namespace Pandora.Cigfi.IServices.Sys
{
    public class AdminLogService : BaseRepository<Sys_AdminLogModel>, IAdminLogService
    {
        public AdminLogService(IDapperRepository context) : base(context)
        {


        }
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        public async Task<IEnumerable<Sys_AdminLogModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"select * from  {TableNameConst.SYS_ADMINLOG}");
            var parameters = new DynamicParameters();
            GetWhere(sqlBuilder, queryHt, parameters);
            sqlBuilder.Append($" order by Id desc  limit {(page - 1) * limit} ,{limit}");

            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<Sys_AdminLogModel>(sqlBuilder.ToString(), parameters);

            }
        }



        public async Task<int> CountAsync(Hashtable queryHt)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append($"select count(*) from  {TableNameConst.SYS_ADMINLOG}");
            var parameters = new DynamicParameters();
            GetWhere(sqlBuilder, queryHt, parameters);
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sqlBuilder.ToString(), parameters);
            }
        }
        private void GetWhere(StringBuilder sqlBuilder,Hashtable queryHt, DynamicParameters parameters)
        {
            var whereList = new List<string>();

            var keyword = queryHt["keyword"];
            if (null!=keyword)
            {
                whereList.Add($"LogMsg LIKE @Keyword");
                parameters.Add("Keyword", $"%{keyword}%");
            }
            var logip = queryHt["logip"];
            if (null != logip)
            {
                whereList.Add($"LogIP LIKE @logip");
                parameters.Add("logip", $"%{logip}%");
            }
            var userName = queryHt["userName"];
            if (null != userName)
            {
                whereList.Add($"UserName=@userName");
                parameters.Add("userName", userName);
            }
            var beginTime = queryHt["beginTime"];
            if (null != beginTime)
            {
                whereList.Add($"LogTime >= @beginTime");
                parameters.Add("beginTime", beginTime);
            }
            var endTime = queryHt["endTime"];
            if (null != endTime)
            {
                whereList.Add($"LogTime<=@endTime");
                parameters.Add("endTime", endTime);
            }
            if (whereList.Count>0)
            {
                sqlBuilder.Append(string.Concat(" where ", string.Join(" and ", whereList)));
            }
        }


        /// <summary>
        /// 登录失败的次数
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="beginLoginTime">开始计算的登录时间</param>
        /// <returns></returns>
        public async Task<int> CountFailLoginTimeAsync(string ip, DateTime beginLoginTime)
        {

            string sql = string.Format("select count(*) from  {0} where  LogType=@LogType and LogIP=@LogIP and LogTime>=@LogTime ", TableNameConst.SYS_ADMINLOG);
            var parameters = new DynamicParameters();
            parameters.Add("LogIP", ip);
            parameters.Add("LogTime", DateTime.Now.AddMinutes(-15));
            parameters.Add("LogType",Convert.ToInt16(AdminLogType.LOGINFAIL));
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
        }
    }


    }
