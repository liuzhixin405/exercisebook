using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 数据播报-持仓总量统计
    /// </summary>
    public partial class BqDataBroadCastStat
    {
        public int Id { get; set; }
        /// <summary>
        /// 买多持仓总量
        /// </summary>
        public decimal BuyHoldCount { get; set; }
        /// <summary>
        /// 精英买多持仓总量
        /// </summary>
        public decimal EliteBuyHoldCount { get; set; }
        /// <summary>
        /// 精英买多持仓比例
        /// </summary>
        public decimal EliteBuyRatio { get; set; }
        /// <summary>
        /// 精英卖空持仓总量
        /// </summary>
        public decimal EliteSellHoldCount { get; set; }
        /// <summary>
        /// 精英卖空持仓比例
        /// </summary>
        public decimal EliteSellRatio { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string FCode { get; set; } = null!;
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 卖空持仓总量
        /// </summary>
        public decimal SellHoldCount { get; set; }
        /// <summary>
        /// 较上一日变化量（正数代表增加，负数代表减少）
        /// </summary>
        public decimal TotalChange { get; set; }
    }
}
