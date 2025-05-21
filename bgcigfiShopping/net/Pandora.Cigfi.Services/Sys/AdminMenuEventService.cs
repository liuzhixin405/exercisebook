
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FXH.Common.DapperService;
using Pandora.Cigfi.Services.Sys; 
using Pandora.Cigfi.Models.Consts; 
using Pandora.Cigfi.Models.Sys;
using System.Linq;
namespace Pandora.Cigfi.IServices.Sys
{
    public class AdminMenuEventService : BaseRepository<Sys_AdminMenuEventModel>, IAdminMenuEventService
    {
        public AdminMenuEventService(IDapperRepository context) : base(context)
        {


        }
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        public async Task<IEnumerable<Sys_AdminMenuEventModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = string.Format("select * from  {0} ", TableNameConst.SYS_ADMINMENUEVENT);
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += " order by   Id desc   " ;
            if(limit>0)
            {
                sql += string.Format("  limit {0} ,{1}", (page - 1) * limit, limit);
            }
          
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<Sys_AdminMenuEventModel>(sql, parameters);

            }
        }



        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = string.Format("select count(*) from  {0} ", TableNameConst.SYS_ADMINMENUEVENT);
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
                where = " where  MenuKey LIKE @Keyword  ";
                parameters.Add("Keyword", $"%{queryHt["keyword"].ToString()}%");
            }

            return where;
        }

        /// <summary>
        /// 读取菜单事件列表
        /// </summary>
        /// <returns></returns>
       public async Task<Dictionary<int, List<Sys_AdminMenuEventModel>>> GetMenuEventListAsync()
        {
            string sql = string.Format("select Id,MenuKey, MenuId,EventKey,EventName from {0} ", TableNameConst.SYS_ADMINMENUEVENT);

            List<Sys_AdminMenuEventModel> list = new List<Sys_AdminMenuEventModel>();
            using (var connection = DbContext.Connection)
            {
                list = (await connection.QueryAsync<Sys_AdminMenuEventModel>(sql)).ToList() ; 
            }
          Dictionary <int, List<Sys_AdminMenuEventModel>> menuEventDc = new  Dictionary<int, List<Sys_AdminMenuEventModel>> ();
            if(null!= list)
            {
                foreach(var item in list)
                {
                    if (!menuEventDc.ContainsKey(item.MenuId))
                    {
                        List<Sys_AdminMenuEventModel> eventList = new List<Sys_AdminMenuEventModel>();
                        eventList.Add(item);
                        menuEventDc.Add(item.MenuId, eventList);
                    }
                    else
                    {
                        menuEventDc[item.MenuId].Add(item);
                    }
                }
            }

            return menuEventDc;

        }

        /// <summary>
        /// 读取菜单事件列表 Dictionary<主键id, Sys_AdminMenuEventModel >
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<int,  Sys_AdminMenuEventModel >> GetIdMenuEventDcAsync()
        {
            string sql = string.Format("select * from {0} ", TableNameConst.SYS_ADMINMENUEVENT);

            List<Sys_AdminMenuEventModel> list = new List<Sys_AdminMenuEventModel>();
            using (var connection = DbContext.Connection)
            {
                list = (await connection.QueryAsync<Sys_AdminMenuEventModel>(sql)).ToList();
            }
            Dictionary<int, Sys_AdminMenuEventModel > menuEventDc = new Dictionary<int,  Sys_AdminMenuEventModel >();
            if (null != list)
            {
                foreach (var item in list)
                {
                    if (!menuEventDc.ContainsKey(item.Id))
                    { 
                        menuEventDc.Add(item.Id, item);
                    }
                    
                }
            }

            return menuEventDc;

        }

        /// <summary>
        /// 读取菜单事件列表
        /// </summary>
        /// <param name="menuId">菜单id</param>
        /// <returns></returns>
        public async  Task<List<Sys_AdminMenuEventModel>> GetMenuEventListAsync(int menuId)
        {
            string sql = string.Format("select MenuId,EventKey,EventId,EventName from {0} where MenuId=@MenuId ", TableNameConst.SYS_ADMINMENUEVENT);

            List<Sys_AdminMenuEventModel> list = new List<Sys_AdminMenuEventModel>();
            using (var connection = DbContext.Connection)
            {
                list = (await connection.QueryAsync<Sys_AdminMenuEventModel>(sql,new { MenuId= menuId })).ToList();
            } 
            return list; 
        }
    }


    }
