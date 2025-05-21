/*
 * 作者：feiwa/DESKTOP-CR8KUE4
 * 时间：2018-09-02 11:58:48
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

    [Table(TableNameConst.SYS_TARGETEVENT)]
    /// <summary>目标事件</summary>
    [Serializable]
    [Description("目标事件")]
    public class Sys_TargetEventModel
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
        /// 事件key
        /// </summary>
        [DisplayName("事件key")]
        public String EventKey { get; set; }

        /// <summary>          
        /// 事件名称
        /// </summary>
        [DisplayName("事件名称")]
        public String EventName { get; set; }

        /// <summary>          
        /// 是否禁用
        /// </summary>
        [DisplayName("是否禁用")]
        public Int32 IsDisable { get; set; }

        /// <summary>          
        /// 排序
        /// </summary>
        [DisplayName("排序")]
        public Int32 OrderNo { get; set; }

        #endregion


    }
}