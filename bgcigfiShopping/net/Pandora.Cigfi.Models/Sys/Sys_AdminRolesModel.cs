/*
 * 作者：feiwa/DESKTOP-CR8KUE4
 * 时间：2018-09-01 17:22:44
 * 版权：版权所有 (C) 小号科技 研发团队 2017~2018
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text; 
using Pandora.Cigfi.Models.Consts;

namespace Pandora.Cigfi.Models.Sys
{

    [Table(TableNameConst.SYS_ADMINROLES)]
    /// <summary>管理角色</summary>
    [Serializable]
    [Description("管理角色")]
    public class Sys_AdminRolesModel
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
        /// 角色类型
        /// </summary>
        [DisplayName("角色类型")]
        public Int32 RoleType { get; set; }

        /// <summary>          
        /// 角色名称
        /// </summary>
        [DisplayName("角色名称")]
        public String RoleName { get; set; }

        /// <summary>          
        /// 角色简单介绍
        /// </summary>
        [DisplayName("角色简单介绍")]
        public String RoleDescription { get; set; }

        /// <summary>          
        /// 是否是超级管理员
        /// </summary>
        [DisplayName("是否是超级管理员")]
        public Int32 IsSuperAdmin { get; set; }

        /// <summary>          
        /// 星级
        /// </summary>
        [DisplayName("星级")]
        public Int32 Stars { get; set; }

        /// <summary>          
        /// 是否不允许删除
        /// </summary>
        [DisplayName("是否不允许删除")]
        public Int32 NotAllowDel { get; set; }

        /// <summary>          
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public Int32 OrderNo { get; set; }

        /// <summary>          
        /// 颜色
        /// </summary>
        [DisplayName("颜色")]
        public String Color { get; set; }

        /// <summary>          
        /// 管理菜单
        /// </summary>
        [DisplayName("管理菜单")]
        public String Menus { get; set; }

        /// <summary>          
        /// 权限
        /// </summary>
        [DisplayName("权限")]
        public String Powers { get; set; }

        #endregion


    }
}