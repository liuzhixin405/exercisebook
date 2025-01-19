using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqWorkOrderDetail
    {
        public long Id { get; set; }
        public long FCreateTime { get; set; }
        public string? FDirection { get; set; }
        public string? FFile { get; set; }
        public string? FOrderCode { get; set; }
        public string? FReplyContent { get; set; }
        public long FOrderId { get; set; }
        /// <summary>
        /// 回复人员名称
        /// </summary>
        public string? FUserName { get; set; }
    }
}
