using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class DeliveryNoTradeTime
    {
        public int HId { get; set; }
        /// <summary>
        /// 不交易日时间
        /// </summary>
        public int? HTime { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public short? HType { get; set; }
        public short TransactionId { get; set; }
    }
}
