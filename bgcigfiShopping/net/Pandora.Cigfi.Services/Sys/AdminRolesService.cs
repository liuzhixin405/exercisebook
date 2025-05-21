
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FXH.Common.DapperService;
using Pandora.Cigfi.Services.Sys; 
using Pandora.Cigfi.Models.Consts; 
using Pandora.Cigfi.Models.Sys;

namespace Pandora.Cigfi.IServices.Sys
{
    public class AdminRolesService : BaseRepository<Sys_AdminRolesModel>, IAdminRolesService
    {
        public AdminRolesService(IDapperRepository context) : base(context)
        {


        }
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        public async Task<IEnumerable<Sys_AdminRolesModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = string.Format("select * from  {0} ", TableNameConst.SYS_ADMINROLES);
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += " order by OrderNo, Id desc   " ;
            if(limit>0)
            {
                sql += string.Format("  limit {0} ,{1}", (page - 1) * limit, limit);
            }
          
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<Sys_AdminRolesModel>(sql, parameters);

            }
        }



        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = string.Format("select count(*) from  {0} ", TableNameConst.SYS_ADMINROLES);
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
                where = " where  RoleName LIKE @Keyword  ";
                parameters.Add("Keyword", $"%{queryHt["keyword"].ToString()}%");
            }

            return where;
        }


       
    }


    }
