using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 价格指数,一种币种对应一个价格指数
    /// </summary>
    public partial class BqPriceIndexCache
    {
        public ushort Id { get; set; }
        /// <summary>
        /// 币种id(currency_type:currency_type_id)
        /// </summary>
        public ushort CurrencyTypeId { get; set; }
        /// <summary>
        /// 价格指数
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 用于上证昨日收盘价
        /// </summary>
        public decimal? Otherprice { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public uint Dateline { get; set; }
        public string TypeCode { get; set; } = null!;
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Last { get; set; }
        public decimal? Turnover { get; set; }
        public decimal Vol { get; set; }
        public decimal? Cap { get; set; }
        public decimal? Change1h { get; set; }
        public decimal? Change24h { get; set; }
        public decimal? Change7d { get; set; }
        public string? TransPair { get; set; }
        public string Market { get; set; } = null!;
        public decimal Ratio { get; set; }
    }
}
