using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqNotradetime
    {
        public long HId { get; set; }
        /// <summary>
        /// 不交易日时间
        /// </summary>
        public long HTime { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public short? HType { get; set; }
        public long TransactionId { get; set; }
    }
}
