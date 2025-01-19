using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 记录经纪人奖励总统计汇总
    /// </summary>
    public partial class BqInviteAwardCount
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 单日奖励总金额
        /// </summary>
        public decimal DayCount { get; set; }
        /// <summary>
        /// 当周奖励总金额
        /// </summary>
        public decimal WeekCount { get; set; }
        /// <summary>
        /// 当月奖励总金额
        /// </summary>
        public decimal MonthCount { get; set; }
        /// <summary>
        /// 账户奖励总金额
        /// </summary>
        public decimal TotalCount { get; set; }
        /// <summary>
        /// 当天时间，unix时间格式，只保存yyyy-mm-dd这部分的信息
        /// </summary>
        public uint Day { get; set; }
    }
}
