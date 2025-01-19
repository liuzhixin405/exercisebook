using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqTransNoRelation
    {
        public string TransNo { get; set; } = null!;
        public long RecordId { get; set; }
        public int TransType { get; set; }
    }
}
