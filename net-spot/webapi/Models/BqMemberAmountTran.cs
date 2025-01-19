using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberAmountTran
    {
        public long RecordId { get; set; }
        public decimal? Amount { get; set; }
        public long Dateline { get; set; }
        public long FUserId { get; set; }
        public string? Title { get; set; }
        public long TradeId { get; set; }
    }
}
