using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 兑换功能设置
    /// </summary>
    public partial class BqExchangeSetting
    {
        public uint Id { get; set; }
        /// <summary>
        /// 币种id
        /// </summary>
        public int CurrencyTypeId { get; set; }
        /// <summary>
        /// 币种符号（大写）
        /// </summary>
        public string Symbol { get; set; } = null!;
        /// <summary>
        /// ckusd兑换该币种所冻结幅度（%）
        /// </summary>
        public decimal FrozenRate { get; set; }
        /// <summary>
        /// 该币种兑换ckusd手续（%）
        /// </summary>
        public decimal FeeRateToUsdt { get; set; }
        /// <summary>
        /// ckusd兑换该币种手续费（%）
        /// </summary>
        public decimal FeeRateFromUsdt { get; set; }
        public decimal OutsideBuyRange { get; set; }
        public decimal OutsideSellRange { get; set; }
        public short Digits { get; set; }
        public decimal MinAmount { get; set; }
        /// <summary>
        /// 外部对冲状态(0-停止，1-开启)
        /// </summary>
        public ulong Running { get; set; }
        /// <summary>
        /// 内部兑换
        /// </summary>
        public ulong InsideExchange { get; set; }
        /// <summary>
        /// 汇率（内部兑换使用）
        /// </summary>
        public decimal ExchangeRate { get; set; }
    }
}
