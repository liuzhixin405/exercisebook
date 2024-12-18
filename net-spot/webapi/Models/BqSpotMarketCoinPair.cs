using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 现货交易对(币币交易)
    /// </summary>
    public partial class BqSpotMarketCoinPair
    {
        public long Id { get; set; }
        /// <summary>
        /// 交易对名称
        /// </summary>
        public string CoinPairName { get; set; } = null!;
        /// <summary>
        /// 交易对币种
        /// </summary>
        public string CoinName { get; set; } = null!;
        /// <summary>
        /// 交易对币种id
        /// </summary>
        public int CoinId { get; set; }
        /// <summary>
        /// 交易对状态(1:正常 2:关闭 3:其他)
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 最新价格
        /// </summary>
        public decimal LatestPrice { get; set; }
        /// <summary>
        /// 最高价格
        /// </summary>
        public decimal HighestPrice { get; set; }
        /// <summary>
        /// 最低价格
        /// </summary>
        public decimal LowestPrice { get; set; }
        /// <summary>
        /// 交易区id
        /// </summary>
        public short MarketId { get; set; }
        /// <summary>
        /// 交易区币种id
        /// </summary>
        public int MarketCoinId { get; set; }
        /// <summary>
        /// 交易区币种名称
        /// </summary>
        public string MarketCoinName { get; set; } = null!;
        /// <summary>
        /// 交易对币种买入费比例
        /// </summary>
        public decimal CoinBuyRate { get; set; }
        /// <summary>
        /// 交易对币种卖出费比例
        /// </summary>
        public decimal CoinSellRate { get; set; }
        /// <summary>
        /// 最小交易数量
        /// </summary>
        public decimal MinTradeNums { get; set; }
        /// <summary>
        /// 最小浮动价格
        /// </summary>
        public decimal MinFloatPrice { get; set; }
        /// <summary>
        /// 交易数量保留小数位数
        /// </summary>
        public string Decimals { get; set; } = null!;
        /// <summary>
        /// 是否暂停
        /// </summary>
        public ulong Suspend { get; set; }
    }
}
