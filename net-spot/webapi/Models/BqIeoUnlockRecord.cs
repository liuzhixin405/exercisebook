using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// IEO账户资金解锁记录
    /// </summary>
    public partial class BqIeoUnlockRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 当日挖矿总量
        /// </summary>
        public decimal? TotalMine { get; set; }
        /// <summary>
        /// 解锁比率
        /// </summary>
        public decimal UnlockRate { get; set; }
        /// <summary>
        /// 解锁量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string? Day { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
    }
}
