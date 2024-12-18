using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// bfx抵扣手续费记录表
    /// </summary>
    public partial class BqBfxFeeRecord
    {
        public int Id { get; set; }
        public short CoinId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string FCode { get; set; } = null!;
        /// <summary>
        /// 合约分区
        /// </summary>
        public string Part { get; set; } = null!;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public int TradeTime { get; set; }
        /// <summary>
        /// 抵扣的手续费(USDT)
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 扣除金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// bfx价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }
    }
}
