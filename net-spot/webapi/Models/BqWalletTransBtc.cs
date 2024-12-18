using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqWalletTransBtc
    {
        public long FTransId { get; set; }
        public string? FWalletId { get; set; }
        public string? FCategory { get; set; }
        public int? FTransType { get; set; }
        public string? FAccount { get; set; }
        public string? FAddress { get; set; }
        public decimal? FCredit { get; set; }
        public decimal? FDebit { get; set; }
        public decimal? FDebitFee { get; set; }
        public decimal? FAmount { get; set; }
        public decimal? FBalance { get; set; }
        public int? FConfirms { get; set; }
        public string? FTxid { get; set; }
        public int? FTime { get; set; }
        public int? FTimereceived { get; set; }
        public int? FState { get; set; }
        public string? FComment1 { get; set; }
        public string? FComment2 { get; set; }
        public string? FComment3 { get; set; }
        public string? FComment4 { get; set; }
        public string? FComment5 { get; set; }
    }
}
