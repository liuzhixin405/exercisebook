/*
 * 作者：feiwa/DESKTOP-CR8KUE4
 * 时间：2018-09-02 11:47:16
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

namespace Pandora.Cigfi.Models.Sys
{

    [Table(TableNameConst.SYS_ADMINMENUEVENT)]
    /// <summary>后台菜单对应的事件权限</summary>
    [Serializable]
    [Description("后台菜单对应的事件权限")]
    public class Sys_AdminMenuEventModel
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
        /// 菜单ID
        /// </summary>
        [DisplayName("菜单ID")]
        public Int32 MenuId { get; set; }

        /// <summary>          
        /// 菜单key
        /// </summary>
        [DisplayName("菜单key")]
        public String MenuKey { get; set; }

        /// <summary>          
        /// 事件ID
        /// </summary>
        [DisplayName("事件ID")]
        public Int32 EventId { get; set; }

        /// <summary>          
        /// 事件key
        /// </summary>
        [DisplayName("事件key")]
        public String EventKey { get; set; }

        /// <summary>          
        /// 事件名称
        /// </summary>
        [DisplayName("事件名称")]
        public String EventName { get; set; }

        #endregion


    }
}