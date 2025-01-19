using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// IEO账户
    /// </summary>
    public partial class BqIeoAccount
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 币种id
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 初始量（充值-提币）
        /// </summary>
        public decimal Initial { get; set; }
        /// <summary>
        /// 锁仓量
        /// </summary>
        public decimal Locked { get; set; }
        /// <summary>
        /// 解锁量
        /// </summary>
        public decimal Unlocked { get; set; }
    }
}
