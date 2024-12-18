using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 记录经纪人奖励月统计汇总
    /// </summary>
    public partial class BqInviteAwardCountMonth
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 当月奖励汇总金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 月份(数字类型:201405)
        /// </summary>
        public int Month { get; set; }
    }
}
