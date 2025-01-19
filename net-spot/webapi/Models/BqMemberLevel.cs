using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户vip等级表(level_credits值为1000000000的时候，表示付费服务)
    /// </summary>
    public partial class BqMemberLevel
    {
        /// <summary>
        /// 等级ID
        /// </summary>
        public byte LevelId { get; set; }
        /// <summary>
        /// 级别名称
        /// </summary>
        public string LevelName { get; set; } = null!;
        /// <summary>
        /// 等级所需积分
        /// </summary>
        public ulong LevelCredits { get; set; }
        /// <summary>
        /// 期货交易佣金比例
        /// </summary>
        public decimal TransactionCommissionRate { get; set; }
        /// <summary>
        /// 经纪人返佣金比例
        /// </summary>
        public decimal BrokerCommissionRate { get; set; }
        /// <summary>
        /// 虚拟货币提现手续费
        /// </summary>
        public decimal WithdrawsCash { get; set; }
        /// <summary>
        /// 电话委托(-1:不支持 0:不限次数 &gt;0支持n次/每)
        /// </summary>
        public bool DelegationTel { get; set; }
        public decimal PendingCommissionRate { get; set; }
        public decimal EatCommissionRate { get; set; }
        public short State { get; set; }
        /// <summary>
        /// 购买VIP所需具体金额，0表示不能购买和使用
        /// </summary>
        public decimal LevelBuyAmount { get; set; }
    }
}
