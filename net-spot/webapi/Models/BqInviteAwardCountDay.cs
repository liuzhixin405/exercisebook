using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 记录经纪人奖励日统计汇总
    /// </summary>
    public partial class BqInviteAwardCountDay
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 当日奖励汇总金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 当天日期(yyyy-mm-dd)
        /// </summary>
        public DateOnly Day { get; set; }
    }
}
