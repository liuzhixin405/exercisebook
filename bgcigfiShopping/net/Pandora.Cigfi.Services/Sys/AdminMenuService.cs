
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
using System.Data;
using Dapper.Contrib.Extensions;

namespace Pandora.Cigfi.IServices.Sys
{
    public class AdminMenuService : BaseRepository<Sys_AdminMenuModel>, IAdminMenuService
    {
        public AdminMenuService(IDapperRepository context) : base(context)
        {


        }
        /// <summary>
        /// 是否存在相同menuKey值的记录
        /// </summary>
        /// <param name="menuKey"></param>
        /// <param name="menuId">不等于此menuId</param>
        public async Task<bool> ExistsAsync(string menuKey, int id = 0)
        {
            string sql = string.Format("select count(*) from  {0} where MenuKey=@MenuKey ", TableNameConst.SYS_ADMINMENU);
            if (id == 0)
            {
                sql += " AND Id!=@id ";
            }

            using (var connection = DbContext.Connection)
            {
                if (id > 0)
                {
                    return await connection.ExecuteScalarAsync<int>(sql, new { MenuKey = menuKey }) > 0;
                }
                else
                {
                    return await connection.ExecuteScalarAsync<int>(sql, new { MenuKey = menuKey, Id = id }) > 0;

                }
            }
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="menuEventList"></param>
        /// <returns></returns>
        public async Task<bool> InsertMenuAsync(Sys_AdminMenuModel model, List<Sys_AdminMenuEventModel> menuEventList)
        { 
            using (var connection = DbContext.Connection)
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                 int menuID=await    connection.InsertAsync<Sys_AdminMenuModel>(model, transaction);
                    foreach (var menuEventItem in menuEventList)
                    {
                        menuEventItem.MenuId = menuID;
                        await connection.InsertAsync<Sys_AdminMenuEventModel>(menuEventItem, transaction);
                    }
                    transaction.Commit();
                    return true;
                }
                catch (System.Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="menuEventList"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMenuAsync(Sys_AdminMenuModel model, List<Sys_AdminMenuEventModel> menuEventList)
        {
            using (var connection = DbContext.Connection)
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                     await connection.UpdateAsync<Sys_AdminMenuModel>(model, transaction);
                    string sql = string.Format(" delete from {0} where MenuId={1} ",TableNameConst.SYS_ADMINMENUEVENT, model.Id);
                    await connection.ExecuteAsync(sql, transaction: transaction);

                    foreach (var menuEventItem in menuEventList)
                    {
                       
                        await connection.InsertAsync<Sys_AdminMenuEventModel>(menuEventItem, transaction);
                    }
                    transaction.Commit();
                    return true;
                }
                catch (System.Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        public async Task<IEnumerable<Sys_AdminMenuModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = string.Format("select * from  {0} ", TableNameConst.SYS_ADMINMENU);
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += string.Format(" order by OrderNo, Id desc  limit {0} ,{1}", (page - 1) * limit, limit);

            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<Sys_AdminMenuModel>(sql, parameters);

            }
        }



        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = string.Format("select count(*) from  {0} ", TableNameConst.SYS_ADMINMENU);
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
                if(queryHt.Contains("keyword"))
                {
                    where = "    MenuName LIKE @Keyword  ";
                    parameters.Add("Keyword", $"%{queryHt["keyword"].ToString()}%");
                }
                if (queryHt.Contains("pid"))
                {
                    if (where.Length > 0)
                    {
                        where +=" and ";
                    }
                    where = "    PId=@PId  ";
                    parameters.Add("PId",  queryHt["pid"].ToString() );
                }
            }

            if(where.Length>0)
            {
                where  = " where "+ where;
            }

            return where;
        }
        /// <summary>
        /// 读取网站左侧导航栏菜单
        /// </summary>
        /// <returns></returns>
        public async Task<List<Sys_AdminMenuViewModel>> GetLeftMenuAsync()
        {
            int pid = 0;
           int maxLevel = -1;
            bool showhide = false;
           

            var menuList = (await base.GetAllAsync()).ToList();
            List<Sys_AdminMenuViewModel> listTree = new List<Sys_AdminMenuViewModel>();
            List<Sys_AdminMenuModel> list = FindByParentID(menuList, pid);
            if (list != null && list.Count > 0)
            {
                foreach (Sys_AdminMenuModel parentMenu in list)
                {
                    if (!showhide && parentMenu.IsHide == 1)
                    {
                        continue;
                    }
                    else
                    {
                        Sys_AdminMenuViewModel viewModel = new Sys_AdminMenuViewModel();
                        viewModel.Menu = parentMenu;
                        if ((maxLevel == -1 || parentMenu.Level < maxLevel) && FindByParentID(menuList, parentMenu.Id).Count > 0)
                        {
                           List<Sys_AdminMenuModel> subMenuList = GetListTree(menuList, parentMenu.Id, maxLevel, false);
                            foreach (Sys_AdminMenuModel subMenu in subMenuList)
                            {
                                if (!showhide && subMenu.IsHide == 1) continue;//如果不显示隐藏，跳过 

                                viewModel.SubMenuList.Add(subMenu);
                            }
                        }
                        if (viewModel.SubMenuList.Count>0)
                        {
                            viewModel.SubMenuList.Sort();
                            listTree.Add(viewModel);
                        }
                        
                    }
                }
            }
            listTree.Sort();
            return listTree;
        }
        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="pid">上级ID，0为全部</param>
        /// <param name="maxLevel">最大级别0为1级类别；-1为所有下级</param>
        /// <param name="isIndentation">是否缩进</param>
        /// <param name="showhide">是否显示隐藏菜单</param>
        /// <returns></returns>
        public async  Task<List<Sys_AdminMenuModel>> GetListTreeAsync(int pid, int maxLevel, bool isIndentation, bool showhide = true)
        {
            var menuList=(await  GetPageListAsync(new Hashtable(),1,1000)).ToList();
            var listTree= GetListTree(menuList, pid, maxLevel, isIndentation, showhide);
            return listTree;
        }
        private  List<Sys_AdminMenuModel>  GetListTree(List<Sys_AdminMenuModel> menuList, int pid, int maxLevel, bool isIndentation, bool showhide = true)
        {
             
            List<Sys_AdminMenuModel> listTree = new List<Sys_AdminMenuModel>();
           List<Sys_AdminMenuModel> list = FindByParentID(menuList,pid);
            if (list != null && list.Count > 0)
            {
                foreach (Sys_AdminMenuModel parentMenu in list)
                {
                    if (!showhide && parentMenu.IsHide == 1)
                    {
                        continue;
                    }
                    else
                    {
                        listTree.Add(parentMenu);
                        if ((maxLevel == -1 || parentMenu.Level < maxLevel) && FindByParentID(menuList,parentMenu.Id).Count > 0)
                        {
                            IList<Sys_AdminMenuModel> listtmp = GetListTree(menuList, parentMenu.Id, maxLevel, false);
                            foreach (Sys_AdminMenuModel subMenu in listtmp)
                            {
                                if (!showhide && subMenu.IsHide == 1) continue;//如果不显示隐藏，跳过
                              
                                if (subMenu.Level > 0 && isIndentation)
                                {
                                    subMenu.MenuName = new string('　', subMenu.Level * 2) + subMenu.MenuName;
                                }
                                listTree.Add(subMenu);
                            }
                        }
                    }
                }
            }
            
            return listTree;


        }
        private   List<Sys_AdminMenuModel> FindByParentID(List<Sys_AdminMenuModel> menuList, Int32 pid)
        {

            List<Sys_AdminMenuModel> list = new List<Sys_AdminMenuModel>();
            if(null!= menuList&& menuList.Count>0)
            {
                foreach (var item in menuList)
                {
                    if(pid==item.PId)
                    {
                        list.Add(item);
                    }
                }
            } 

            return list;
        }


    }


}
