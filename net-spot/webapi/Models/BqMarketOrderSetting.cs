using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 市价委托设置
    /// </summary>
    public partial class BqMarketOrderSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 价格阈值范围比例
        /// </summary>
        public decimal PriceRange { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public ulong Running { get; set; }
    }
}
