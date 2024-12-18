using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class VTransactionRate
    {
        public long Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 该合约价格指数的上波动比例
        /// </summary>
        public decimal? Uprate { get; set; }
        /// <summary>
        /// 该合约价格指数的下波动比例
        /// </summary>
        public decimal? Downrate { get; set; }
    }
}
