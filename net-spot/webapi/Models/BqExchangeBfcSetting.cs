using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// BFX与BFC互兑设置
    /// </summary>
    public partial class BqExchangeBfcSetting
    {
        public uint Id { get; set; }
        /// <summary>
        /// 币种id
        /// </summary>
        public int CurrencyTypeId { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string Symbol { get; set; } = null!;
        /// <summary>
        /// 兑换价格（后台管理设置）
        /// </summary>
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// 最小兑换数量
        /// </summary>
        public decimal MinAmount { get; set; }
        /// <summary>
        /// 是否开启兑换（默认开启）
        /// </summary>
        public ulong Running { get; set; }
    }
}
