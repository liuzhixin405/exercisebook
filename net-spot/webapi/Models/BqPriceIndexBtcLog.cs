using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// btc 币的价格指数记录日志表，每一种币种使用一个独立的指数表记录
    /// </summary>
    public partial class BqPriceIndexBtcLog
    {
        public uint Id { get; set; }
        /// <summary>
        /// 当前指数价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 币种id
        /// </summary>
        public short CurrencyTypeId { get; set; }
    }
}
