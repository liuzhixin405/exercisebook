using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 交易收益统计表
    /// </summary>
    public partial class TradeLogIncomeReconcilium
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 盈利额
        /// </summary>
        public decimal Income { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        public int CutOffTime { get; set; }
    }
}
