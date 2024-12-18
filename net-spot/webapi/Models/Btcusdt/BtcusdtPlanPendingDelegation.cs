using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 计划委托表
    /// </summary>
    public partial class BtcusdtPlanPendingDelegation
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string FCode { get; set; } = null!;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 触发价格
        /// </summary>
        public decimal TriPrice { get; set; }
        /// <summary>
        /// 委托价格
        /// </summary>
        public decimal DelegationPrice { get; set; }
        /// <summary>
        /// 委托数量
        /// </summary>
        public decimal DelegationCount { get; set; }
        /// <summary>
        /// 委托时间
        /// </summary>
        public int DelegationTime { get; set; }
        /// <summary>
        /// 杆杠倍数
        /// </summary>
        public short Multiple { get; set; }
        /// <summary>
        /// 操作方向
        /// </summary>
        public short Direction { get; set; }
        /// <summary>
        /// 类型（1-现价大于触发价时进行委托，2-现价低于触发价时进行委托）
        /// </summary>
        public short Type { get; set; }
        /// <summary>
        /// 状态（0-未生效，1-已生效，2-失败，3-已撤销）
        /// </summary>
        public short Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
    }
}
