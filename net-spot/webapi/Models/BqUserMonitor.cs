using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 被监控的用户表
    /// </summary>
    public partial class BqUserMonitor
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 资金余额低于多少时平仓
        /// </summary>
        public decimal Threshold { get; set; }
        /// <summary>
        /// 平仓价格比率
        /// </summary>
        public decimal PriceRate { get; set; }
    }
}
