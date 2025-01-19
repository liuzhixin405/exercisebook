using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqSpotBetterTokenTrade
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public decimal AveragePrice { get; set; }
        public int BrokerId { get; set; }
        public long CreateOn { get; set; }
        public decimal DealAmount { get; set; }
        public decimal? DealQuoteAmount { get; set; }
        public decimal EntrustPrice { get; set; }
        /// <summary>
        /// 是否自成交
        /// </summary>
        public bool IsSelf { get; set; }
        public decimal? NotStrike { get; set; }
        public decimal? OpenAmount { get; set; }
        public long OrderId { get; set; }
        public string PairCode { get; set; } = null!;
        public decimal? QuoteAmount { get; set; }
        public string Side { get; set; } = null!;
        public string? SourceInfo { get; set; }
        public short Status { get; set; }
        public string Symbol { get; set; } = null!;
        public string SystemOrderType { get; set; } = null!;
        public decimal TrunOver { get; set; }
        public long UpdateOn { get; set; }
        public int UserId { get; set; }
    }
}
