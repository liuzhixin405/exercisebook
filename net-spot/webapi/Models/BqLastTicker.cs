using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqLastTicker
    {
        public uint FTickerId { get; set; }
        public string FTickerCode { get; set; } = null!;
        public decimal FTickerHigh { get; set; }
        public decimal FTickerLow { get; set; }
        public decimal FTickerBuy { get; set; }
        public decimal FTickerSell { get; set; }
        public decimal FTickerLast { get; set; }
        public decimal FTickerVol { get; set; }
        public decimal FTickerTrades { get; set; }
    }
}
