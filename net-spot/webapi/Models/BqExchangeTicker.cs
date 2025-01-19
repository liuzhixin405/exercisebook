using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqExchangeTicker
    {
        public uint Id { get; set; }
        public ushort CurrencyTypeId { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Last { get; set; }
        public decimal Buy { get; set; }
        public decimal Sell { get; set; }
        public decimal Vol { get; set; }
        public int Dateline { get; set; }
        public string Market { get; set; } = null!;
        public string TransPair { get; set; } = null!;
    }
}
