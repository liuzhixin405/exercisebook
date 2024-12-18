using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class TradeLog
    {
        public long TradeId { get; set; }
        public long FUserId { get; set; }
        public decimal? TradeAveragePrice { get; set; }
        public decimal? TradeCount { get; set; }
        public decimal? TradeDeposit { get; set; }
        public decimal? TradeFee { get; set; }
        public decimal? TradeIncome { get; set; }
        public int TradeMultiple { get; set; }
        public long TradeTime { get; set; }
        public int TradeTypeId { get; set; }
        /// <summary>
        /// 这毛交易的成交方向
        /// </summary>
        public byte TradeTransDirction { get; set; }
        public string? TransactionCode { get; set; }
        public long TransactionId { get; set; }
    }
}
