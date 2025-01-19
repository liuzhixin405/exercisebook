using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 周度模拟交易排名表
    /// </summary>
    public partial class VTradeWeekStat
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// 保证金总额
        /// </summary>
        public decimal? SumDepoist { get; set; }
        /// <summary>
        /// 盈亏总额
        /// </summary>
        public decimal? SumIncome { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 收益率（%）
        /// </summary>
        public decimal? YieldRate { get; set; }
        /// <summary>
        /// 周度开始时间
        /// </summary>
        public int? RecordTime { get; set; }
    }
}
