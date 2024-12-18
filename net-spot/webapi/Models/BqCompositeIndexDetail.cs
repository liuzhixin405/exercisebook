using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 综合指数配比详情
    /// </summary>
    public partial class BqCompositeIndexDetail
    {
        public int Id { get; set; }
        /// <summary>
        /// 综合指数类型
        /// </summary>
        public short IndexType { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 最新指数价
        /// </summary>
        public decimal Last { get; set; }
        /// <summary>
        /// 市场来源
        /// </summary>
        public string Market { get; set; } = null!;
        /// <summary>
        /// 最新价更新时间
        /// </summary>
        public uint Dateline { get; set; }
    }
}
