using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 币币交易每日统计报表
    /// </summary>
    public partial class SpotStatisticalForm
    {
        public long Id { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public int StatisticalTime { get; set; }
        /// <summary>
        /// 所有交易对的交易总人数
        /// </summary>
        public int TotalTradeUser { get; set; }
        /// <summary>
        /// BFX抵扣手续费总额
        /// </summary>
        public decimal TotalBfxFee { get; set; }
        /// <summary>
        /// 市场币种返利总额（卖出返利）
        /// </summary>
        public decimal TotalMarketFee { get; set; }
        /// <summary>
        /// 本币手续费总额
        /// </summary>
        public decimal TotalCoinRebate { get; set; }
        /// <summary>
        /// 市场币种返利总额
        /// </summary>
        public decimal TotalMarketRebate { get; set; }
        /// <summary>
        /// bfx抵扣返佣总额
        /// </summary>
        public decimal TotalBfxRebate { get; set; }
        /// <summary>
        /// 交易对
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// 市场币种名称
        /// </summary>
        public string Market { get; set; } = null!;
        /// <summary>
        /// 本币种名称
        /// </summary>
        public string Coin { get; set; } = null!;
        /// <summary>
        /// 真实交易额
        /// </summary>
        public decimal TradeAmount { get; set; }
        /// <summary>
        /// 单个交易对交易人数
        /// </summary>
        public int TradeUser { get; set; }
        /// <summary>
        /// 买入手续费
        /// </summary>
        public decimal BuyTradeFee { get; set; }
        /// <summary>
        /// 卖出手续费
        /// </summary>
        public decimal SellTradeFee { get; set; }
        /// <summary>
        /// BFX抵扣买入手续费
        /// </summary>
        public decimal BuyTradeBfxFee { get; set; }
        /// <summary>
        /// BFX抵扣卖出手续费
        /// </summary>
        public decimal SellTradeBfxFee { get; set; }
        /// <summary>
        /// 本币种返利（买入返利）
        /// </summary>
        public decimal CoinRebate { get; set; }
        /// <summary>
        /// 市场币种返利（卖出返利）
        /// </summary>
        public decimal MarketRebate { get; set; }
        /// <summary>
        /// bfx返利
        /// </summary>
        public decimal BfxRebate { get; set; }
    }
}
