using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 成交盈亏及手续费统计
    /// </summary>
    public partial class DeliveryTradeLogReconcilium
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 时间,每月1号0点,unix时间格式
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal TradeFee { get; set; }
        /// <summary>
        /// 盈亏额
        /// </summary>
        public decimal TradeIncome { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 合约id
        /// </summary>
        public short TransactionId { get; set; }
    }
}
