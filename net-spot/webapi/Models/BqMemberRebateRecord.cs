using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// vip会员返佣记录（交易除外）
    /// </summary>
    public partial class BqMemberRebateRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 返利币种id
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 返利币种名称
        /// </summary>
        public string CoinName { get; set; } = null!;
        /// <summary>
        /// 返佣比例
        /// </summary>
        public decimal CommissionRate { get; set; }
        /// <summary>
        /// 产生返佣的动作（ 1-购买vip返佣）
        /// </summary>
        public short RebateType { get; set; }
        /// <summary>
        /// 用户vip等级
        /// </summary>
        public short UserLevel { get; set; }
        /// <summary>
        /// 会员账户id
        /// </summary>
        public int SubUserId { get; set; }
        /// <summary>
        /// 返佣金额
        /// </summary>
        public decimal RebateAmount { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
    }
}
