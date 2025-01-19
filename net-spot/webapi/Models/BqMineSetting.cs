using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 平台挖矿相关设置
    /// </summary>
    public partial class BqMineSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 挖矿币种
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 挖矿总量
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 平台已挖数量
        /// </summary>
        public decimal MinedAmount { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public ulong Running { get; set; }
        /// <summary>
        /// 邀请返佣是否开启
        /// </summary>
        public ulong Rebate { get; set; }
        public int BeginTime { get; set; }
        /// <summary>
        /// 个人挖矿数量上限
        /// </summary>
        public decimal PersonalMaxAmount { get; set; }
    }
}
