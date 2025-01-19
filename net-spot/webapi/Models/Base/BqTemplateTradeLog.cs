using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 交易记录日志表。一个交易会产生两个用户对这个交易的操作记录，基本一份是更新，另一份是写入新记录
    /// </summary>
    public partial class BqTemplateTradeLog
    {
        /// <summary>
        /// 交易ID
        /// </summary>
        public uint TradeId { get; set; }
        /// <summary>
        /// 合约号id
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 成交类型(1:多单开仓/2:多单平仓/3:空头开仓/4:空头平仓/5:多单爆仓/6:空单爆仓)
        /// </summary>
        public byte TradeTypeId { get; set; }
        /// <summary>
        /// 这笔交易的成交方向
        /// </summary>
        public byte TradeTransDirction { get; set; }
        /// <summary>
        /// 成交均价
        /// </summary>
        public decimal TradeAveragePrice { get; set; }
        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal TradeCount { get; set; }
        /// <summary>
        /// 杠杆
        /// </summary>
        public ushort? TradeMultiple { get; set; }
        /// <summary>
        /// 保证金
        /// </summary>
        public decimal TradeDeposit { get; set; }
        /// <summary>
        /// 盈亏额
        /// </summary>
        public decimal TradeIncome { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal TradeFee { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 成交时间,unix时间格式
        /// </summary>
        public uint TradeTime { get; set; }
        public long CoinPairId { get; set; }
        public decimal? TradeBfxFee { get; set; }
        public long CoinId { get; set; }
        public long MarketCoinId { get; set; }
        public decimal TradeAmount { get; set; }
        public long PendingNo { get; set; }
    }
}
