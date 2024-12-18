using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 综合指数配比类型
    /// </summary>
    public partial class BqCompositeIndexType
    {
        public short TypeId { get; set; }
        /// <summary>
        /// 是否使用
        /// </summary>
        public bool IsUsing { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; } = null!;
        /// <summary>
        /// 合约类型号(关联综合指数合约类型号)
        /// </summary>
        public string TransactionTypeCode { get; set; } = null!;
        /// <summary>
        /// 基准指数
        /// </summary>
        public decimal BenchmarkIndex { get; set; }
        /// <summary>
        /// 每日0点记录成交价格
        /// </summary>
        public decimal ClosingPrice { get; set; }
    }
}
