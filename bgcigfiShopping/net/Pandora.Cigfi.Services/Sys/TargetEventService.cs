
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
    public class TargetEventService : BaseRepository<Sys_TargetEventModel>, ITargetEventService
    {
        public TargetEventService(IDapperRepository context) : base(context)
        {


        }

        /// <summary>
        /// 是否存在相同eventKey值的记录
        /// </summary>
        /// <param name="eventKey"></param>
        /// <returns></returns>
        public async  Task<bool> ExistsAsync(string eventKey) {
            string sql = string.Format("select count(*) from  {0} where EventKey=@EventKey ", TableNameConst.SYS_TARGETEVENT);
        
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, new { EventKey =eventKey })>0;
            }
        }
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        public async Task<IEnumerable<Sys_TargetEventModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = string.Format("select * from  {0} ", TableNameConst.SYS_TARGETEVENT);
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += " order by   Id desc   " ;
            if(limit>0& page>0)
            {
                sql += string.Format("  limit {0} ,{1}", (page - 1) * limit, limit);
            }
          
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<Sys_TargetEventModel>(sql, parameters);

            }
        }



        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = string.Format("select count(*) from  {0} ", TableNameConst.SYS_TARGETEVENT);
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
                where = " where  EventName LIKE @Keyword  ";
                parameters.Add("Keyword", $"%{queryHt["keyword"].ToString()}%");
            }

            return where;
        }


       
    }


    }
