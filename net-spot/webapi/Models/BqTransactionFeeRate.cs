using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约设置独立手续费率
    /// </summary>
    public partial class BqTransactionFeeRate
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string FCode { get; set; } = null!;
        /// <summary>
        /// 等级ID
        /// </summary>
        public int LevelId { get; set; }
        /// <summary>
        /// 等级名称
        /// </summary>
        public string LevelName { get; set; } = null!;
        /// <summary>
        /// 挂单佣金比例
        /// </summary>
        public decimal PendingCommissionRate { get; set; }
        /// <summary>
        /// 吃单佣金比例
        /// </summary>
        public decimal EatCommissionRate { get; set; }
    }
}
