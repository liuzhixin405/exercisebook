using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class OtherTradesdatum
    {
        public int DId { get; set; }
        public decimal? DAmount { get; set; }
        public decimal? DPrice { get; set; }
        public int? DOid { get; set; }
        public string? DType { get; set; }
        public int? DTime { get; set; }
        /// <summary>
        /// 1 btc100 2 OK
        /// </summary>
        public string? DFlag { get; set; }
    }
}
