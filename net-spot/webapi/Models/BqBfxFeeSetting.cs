using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 平台币交易手续费相关配置
    /// </summary>
    public partial class BqBfxFeeSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 持平台币享vip等级是否开启
        /// </summary>
        public short VipState { get; set; }
        /// <summary>
        /// 抵扣手续费是否开启
        /// </summary>
        public short FeeState { get; set; }
        /// <summary>
        /// 平台币手续费折扣
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 平台币币种Id
        /// </summary>
        public short CoinId { get; set; }
        /// <summary>
        /// bfx前一日均价
        /// </summary>
        public decimal BfxAvgPrice { get; set; }
        /// <summary>
        /// 均价更新时间
        /// </summary>
        public int UpdatePirceTime { get; set; }
    }
}
