using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 记录合约基准价格
    /// </summary>
    public partial class BqBenchmarkPrice
    {
        public int Id { get; set; }
        /// <summary>
        /// 基准价格
        /// </summary>
        public decimal BenchmarkPrice { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 基准时间
        /// </summary>
        public int Dateline { get; set; }
    }
}
