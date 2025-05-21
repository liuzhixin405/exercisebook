using Pandora.Cigfi.Models.Consts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pandora.Cigfi.Models.Sys
{
    [Table(TableNameConst.SYS_REVIEW_LOG)]
    /// <summary>审核日志表</summary>
    [Serializable]
    [Description("审核日志表")]
    public class Sys_ReviewLogModel
    {
        #region 属性
        [Dapper.Contrib.Extensions.Key]
        //自动增长

        /// <summary>          
        /// 自增ID
        /// </summary>
        [DisplayName("自增ID")]
        public Int64 ID { get; set; }

        /// <summary>          
        /// 操作对象类型，1代表虚拟币，2代表交易所，3代表行情
        /// </summary>
        [DisplayName("操作对象类型，1代表虚拟币，2代表交易所，3代表行情")]
        public Int32 TargetType { get; set; }

        /// <summary>          
        /// 对象Code
        /// </summary>
        [DisplayName("对象Code")]
        public String Code { get; set; }

        /// <summary>          
        /// 操作人ID
        /// </summary>
        [DisplayName("操作人ID")]
        public Int32 Operator { get; set; }

        /// <summary>
        /// 操作入口（存当前控制器名）
        /// </summary>
        [DisplayName("操作入口（存当前控制器名）")]
        public string OperateLocation { get; set; }


        /// <summary>          
        /// 操作类型，1代表编辑，2代表新增，3代表删除，4代表审核通过，5代表审核不通过，6代表分配
        /// </summary>
        [DisplayName("操作类型，1代表编辑，2代表新增，3代表删除，4代表审核通过，5代表审核不通过，6代表分配")]
        public Int32 OperateType { get; set; }

        /// <summary>          
        /// 扩展说明，根据实际情况产生内容
        /// </summary>
        [DisplayName("扩展说明，根据实际情况产生内容")]
        public String Remark { get; set; }

        /// <summary>          
        /// 操作时间
        /// </summary>
        [DisplayName("操作时间")]
        public DateTime OpTime { get; set; }

        #endregion

        /// <summary>
        /// 审核日志表操作对象类型
        /// </summary>
        public enum ReviewLogTargetType
        {
            /// <summary>
            /// 虚拟币
            /// </summary>
            COIN = 1,
            /// <summary>
            /// 交易所
            /// </summary>
            EXCHANGE = 2,
            /// <summary>
            /// 行情
            /// </summary>
            MARKET = 3,
        }

        /// <summary>
        /// 审核日志表操作类型
        /// </summary>
        public enum ReviewLogOperateType
        {
            /// <summary>
            /// 编辑
            /// </summary>
            EDIT = 1,
            /// <summary>
            /// 添加
            /// </summary>
            ADD = 2,
            /// <summary>
            /// 删除
            /// </summary>
            DELETE = 3,
            /// <summary>
            /// 审核通过
            /// </summary>
            PASS = 4,
            /// <summary>
            /// 审核不通过
            /// </summary>
            NOPASS = 5,
            /// <summary>
            /// 分配
            /// </summary>
            DISTRIBUTION = 6
        }
    }
}
