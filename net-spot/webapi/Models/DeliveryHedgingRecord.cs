using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class DeliveryHedgingRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 交易id来自trade_log
        /// </summary>
        public int TradeId { get; set; }
        /// <summary>
        /// 交易数量
        /// </summary>
        public decimal TradeAmount { get; set; }
        /// <summary>
        /// 成交价格
        /// </summary>
        public decimal TradePrice { get; set; }
        /// <summary>
        /// 交易方向1-买入，2-卖出（相对深度用户而言）
        /// </summary>
        public short TradeDirection { get; set; }
        /// <summary>
        /// 成交时间
        /// </summary>
        public int TradeTime { get; set; }
        /// <summary>
        /// 与深度用户Id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 对冲方向（1：买入，2：卖出）
        /// </summary>
        public short Direction { get; set; }
        /// <summary>
        /// 对冲价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public int ProcessTime { get; set; }
        /// <summary>
        /// 记录状态(0:未对冲, 1: 已对冲, 2: 失败)
        /// </summary>
        public short State { get; set; }
        public string? Note { get; set; }
        public short SendTimes { get; set; }
    }
}
