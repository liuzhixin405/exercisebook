using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 资金费配置表
    /// </summary>
    public partial class BqFundingFeeSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约名称
        /// </summary>
        public string FCode { get; set; } = null!;
        /// <summary>
        /// 开启状态(0-停止，1-开启)
        /// </summary>
        public ulong Running { get; set; }
        /// <summary>
        /// 执行时间点，逗号隔开（10:00,22:00）
        /// </summary>
        public string RunningTime { get; set; } = null!;
        /// <summary>
        /// 利率差
        /// </summary>
        public decimal InterestRateDiff { get; set; }
        /// <summary>
        /// 除数
        /// </summary>
        public decimal Divisor { get; set; }
        /// <summary>
        /// 限制区间上边界（%）
        /// </summary>
        public decimal ClampUp { get; set; }
        /// <summary>
        /// 限制区间下边界（%）
        /// </summary>
        public decimal ClampDown { get; set; }
        /// <summary>
        /// 固定资金费率是否开启
        /// </summary>
        public ulong FixedFeeOpen { get; set; }
        /// <summary>
        /// 固定资金费率
        /// </summary>
        public decimal FixedFeeRate { get; set; }
    }
}
