using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约类型表
    /// </summary>
    public partial class VTransactionType
    {
        /// <summary>
        /// 合约类型id
        /// </summary>
        public ushort TypeId { get; set; }
        /// <summary>
        /// 合约类型名称
        /// </summary>
        public string TypeName { get; set; } = null!;
        /// <summary>
        /// 合约类型号
        /// </summary>
        public string TypeCode { get; set; } = null!;
        /// <summary>
        /// 合约默认杠杆(1,5,10,20,50)
        /// </summary>
        public string? DefaultMultiple { get; set; }
        /// <summary>
        /// 默认杠杆的状态(1,0,1,2,1)(0不显示,1显示,2禁用)
        /// </summary>
        public string? DefaultMultipleState { get; set; }
        /// <summary>
        /// 默认交易系数(分母)
        /// </summary>
        public decimal? DefaultCoefficient { get; set; }
        /// <summary>
        /// 使用中
        /// </summary>
        public bool IsUsing { get; set; }
        /// <summary>
        /// 指数地址
        /// </summary>
        public string? IndexAddress { get; set; }
        /// <summary>
        /// 指数
        /// </summary>
        public decimal? IndexValue { get; set; }
        /// <summary>
        /// 最小开仓数量
        /// </summary>
        public decimal? MinKcNums { get; set; }
        /// <summary>
        /// 最小浮动价格
        /// </summary>
        public decimal? MinFloatPrice { get; set; }
    }
}
