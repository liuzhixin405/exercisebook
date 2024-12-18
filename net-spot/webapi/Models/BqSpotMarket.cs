using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 交易区表(币币交易)
    /// </summary>
    public partial class BqSpotMarket
    {
        public ushort Id { get; set; }
        /// <summary>
        /// 交易区名称
        /// </summary>
        public string MarketName { get; set; } = null!;
        /// <summary>
        /// 交易区币种
        /// </summary>
        public string CoinName { get; set; } = null!;
        /// <summary>
        /// 交易区币种id
        /// </summary>
        public int CoinId { get; set; }
        /// <summary>
        /// 是否使用
        /// </summary>
        public bool IsUsing { get; set; }
        /// <summary>
        /// 指数地址
        /// </summary>
        public string? IndexAddress { get; set; }
        /// <summary>
        /// 指数值
        /// </summary>
        public decimal? IndexValue { get; set; }
    }
}
