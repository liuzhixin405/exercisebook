using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BtcusdtTradeLog
    {
        public long TradeId { get; set; }
        public long? CoinPairId { get; set; }
        public string? CoinPairName { get; set; }
        public int? TradeTransDirection { get; set; }
        public decimal? TradeAveragePrice { get; set; }
        public decimal? TradeCount { get; set; }
        public decimal? TradeFee { get; set; }
        public decimal? TradeBfxFee { get; set; }
        public long? FUserId { get; set; }
        public int? CoinId { get; set; }
        public int? MarketCoinId { get; set; }
        public decimal? TradeAmount { get; set; }
        public long? TradeTime { get; set; }
        public long? PendingNo { get; set; }
    }
}
