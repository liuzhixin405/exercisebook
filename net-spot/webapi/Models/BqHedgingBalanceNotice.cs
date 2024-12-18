using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 对冲账户资产通知
    /// </summary>
    public partial class BqHedgingBalanceNotice
    {
        public uint Id { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string CurrencyName { get; set; } = null!;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// 设定发送邮件的余额临界值
        /// </summary>
        public decimal Threshold { get; set; }
        /// <summary>
        /// 类型（1、合约账户；2、币币兑换）
        /// </summary>
        public short Type { get; set; }
    }
}
