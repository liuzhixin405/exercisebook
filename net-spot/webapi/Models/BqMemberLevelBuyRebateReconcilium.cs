using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberLevelBuyRebateReconcilium
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string FUserEmail { get; set; } = null!;
        /// <summary>
        /// 总买入额
        /// </summary>
        public decimal TotalBuyAmount { get; set; }
        /// <summary>
        /// 所有会员总锁仓量
        /// </summary>
        public decimal TotalBfxLock { get; set; }
        /// <summary>
        /// 所有会员总解锁量
        /// </summary>
        public decimal TotalBfxUnlock { get; set; }
        /// <summary>
        /// 自己购买vip的锁仓量
        /// </summary>
        public decimal SelfBfxLock { get; set; }
        /// <summary>
        /// 直接上级会员
        /// </summary>
        public int Superior { get; set; }
        /// <summary>
        /// 二级上级（上级的上级）
        /// </summary>
        public int Superior2 { get; set; }
    }
}
