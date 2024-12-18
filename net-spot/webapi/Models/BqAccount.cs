using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户账户金额表
    /// </summary>
    public partial class BqAccount
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public long FUserId { get; set; }
        /// <summary>
        /// 1:btc; 2:ck.usd
        /// </summary>
        public long CurrencyTypeId { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string FUserEmail { get; set; } = null!;
        /// <summary>
        /// btc金额
        /// </summary>
        public decimal Btc { get; set; }
        /// <summary>
        /// 锁定金额
        /// </summary>
        public decimal BtcLocked { get; set; }
        /// <summary>
        /// 最后金额变更,unix时间格式(用户活跃分析)
        /// </summary>
        public long Lastupdate { get; set; }
        public string? FUserName { get; set; }
    }
}
