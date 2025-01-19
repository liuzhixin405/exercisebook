using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqHourlyAccount
    {
        public int Id { get; set; }
        /// <summary>
        /// 币种数量
        /// </summary>
        public decimal CurrencyAmount { get; set; }
        /// <summary>
        /// 币种id
        /// </summary>
        public short CurrencyId { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public int HourTime { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
    }
}
