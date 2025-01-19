using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BtcusdtMemberTradeDetail
    {
        public long Id { get; set; }
        public long? FBuyTradeId { get; set; }
        public long? FBuyUid { get; set; }
        public string? FBuyUserEmail { get; set; }
        public long? FSellUid { get; set; }
        public string? FSellUserEmail { get; set; }
        public long? FSellTradeId { get; set; }
        public int? TradeFlag { get; set; }
        public int? TradeTransDirction { get; set; }
        public decimal? TradePrice { get; set; }
        public decimal? TradeCount { get; set; }
        public long? TradeTime { get; set; }
        public long? CoinPairId { get; set; }
        public string? CoinPairName { get; set; }
    }
}
