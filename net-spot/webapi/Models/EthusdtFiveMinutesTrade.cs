using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class EthusdtFiveMinutesTrade
    {
        public uint Id { get; set; }
        /// <summary>
        /// 交易对名称
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// 最高价
        /// </summary>
        public decimal High { get; set; }
        /// <summary>
        /// 最低价
        /// </summary>
        public decimal Low { get; set; }
        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal First { get; set; }
        /// <summary>
        /// 收盘价
        /// </summary>
        public decimal Last { get; set; }
        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal Vol { get; set; }
        /// <summary>
        /// 时间,unix时间格式
        /// </summary>
        public uint Time { get; set; }
    }
}
