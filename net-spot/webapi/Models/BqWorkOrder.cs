using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqWorkOrder
    {
        public long Id { get; set; }
        public long FCreateTime { get; set; }
        public string? FFile { get; set; }
        public string? FFromOrderState { get; set; }
        public string? FOrderCode { get; set; }
        public string? FOrderType { get; set; }
        public string? FReplyContent { get; set; }
        public string? FTheme { get; set; }
        public string? FToOrderState { get; set; }
        public long FUpdateTime { get; set; }
        public string? FUserEmail { get; set; }
        public long FUserId { get; set; }
    }
}
