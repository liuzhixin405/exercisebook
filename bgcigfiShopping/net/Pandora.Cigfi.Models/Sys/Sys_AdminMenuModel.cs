/*
 * 作者：feiwa/DESKTOP-CR8KUE4
 * 时间：2018-09-01 17:36:38
 * 版权：版权所有 (C) 小号科技 研发团队 2017~2018
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Pandora.Cigfi.Models;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Models.Sys;

namespace Pandora.Cigfi.Models.Sys
{

    [Table(TableNameConst.SYS_ADMINMENU)]
    /// <summary>后台菜单</summary>
    [Serializable]
    [Description("后台菜单")]
    public class Sys_AdminMenuModel : IComparable
    {
        #region 属性
        [Dapper.Contrib.Extensions.Key]
        //自动增长

        /// <summary>          
        /// 编号
        /// </summary>
        [DisplayName("编号")]
        public Int32 Id { get; set; }

        /// <summary>          
        /// 标识key
        /// </summary>
        [DisplayName("标识key")]
        public String MenuKey { get; set; }

        /// <summary>          
        /// 页面名称
        /// </summary>
        [DisplayName("页面名称")]
        public String MenuName { get; set; }

        /// <summary>          
        /// 页面名称
        /// </summary>
        [DisplayName("页面名称")]
        public String PermissionKey { get; set; }

        /// <summary>          
        /// 介绍
        /// </summary>
        [DisplayName("介绍")]
        public String Description { get; set; }

        /// <summary>          
        /// 页面连接地址
        /// </summary>
        [DisplayName("页面连接地址")]
        public String Link { get; set; }

        /// <summary>          
        /// 上级ID
        /// </summary>
        [DisplayName("上级ID")]
        public Int32 PId { get; set; }

        /// <summary>          
        /// 级别
        /// </summary>
        [DisplayName("级别")]
        public Int32 Level { get; set; }

        /// <summary>          
        /// 路径
        /// </summary>
        [DisplayName("路径")]
        public String Location { get; set; }

        /// <summary>          
        /// 是否隐藏
        /// </summary>
        [DisplayName("是否隐藏")]
        public Int32 IsHide { get; set; }

        /// <summary>          
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public Int32 OrderNo { get; set; }

        /// <summary>          
        /// 图标
        /// </summary>
        [DisplayName("图标")]
        public String Icon { get; set; }

        /// <summary>          
        /// 样式名称
        /// </summary>
        [DisplayName("样式名称")]
        public String ClassName { get; set; }

        #endregion

        public int CompareTo(object obj)
        {
            Sys_AdminMenuModel others = (Sys_AdminMenuModel)obj;
            if (this.OrderNo > others.OrderNo)
            {
                return 1;
            }
            else if (this.OrderNo < others.OrderNo)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
          
    }


    public class Sys_AdminMenuViewModel : IComparable
    {
        public Sys_AdminMenuViewModel()
        {
            Menu = new Sys_AdminMenuModel();
            SubMenuList = new  List<Sys_AdminMenuModel>();
        }
        /// <summary>
        /// 一级菜单
        /// </summary>
        public  Sys_AdminMenuModel  Menu
        {
            get;
            set;
        }
        /// <summary>
        /// 二级菜单
        /// </summary>
        public List<Sys_AdminMenuModel> SubMenuList
        {
            get;
            set;
        }
        public int CompareTo(object obj)
        {
            Sys_AdminMenuViewModel others = (Sys_AdminMenuViewModel)obj;
            if (this.Menu.OrderNo > others.Menu.OrderNo)
            {
                return 1;
            }
            else if (this.Menu.OrderNo < others.Menu.OrderNo)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}