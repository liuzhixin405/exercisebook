using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 标记价格设置
    /// </summary>
    public partial class BqMarkPriceSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// 是否启用
        /// </summary>
        public ulong Running { get; set; }
        /// <summary>
        /// 是否启用固定价格
        /// </summary>
        public ulong Fixed { get; set; }
        /// <summary>
        /// 移动周期
        /// </summary>
        public int MovingPeriod { get; set; }
        /// <summary>
        /// 盘口宽度过滤
        /// </summary>
        public decimal OpenWidthFilter { get; set; }
        /// <summary>
        /// 指数浮动范围
        /// </summary>
        public decimal IndexRateFilter { get; set; }
        /// <summary>
        /// 固定价格值
        /// </summary>
        public decimal FixedPrice { get; set; }
    }
}
