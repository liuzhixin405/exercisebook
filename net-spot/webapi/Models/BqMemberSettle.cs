using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberSettle
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        public int FSettleTime { get; set; }
        /// <summary>
        /// 结算时价格
        /// </summary>
        public decimal FSettlePrice { get; set; }
        /// <summary>
        /// 结算的数量
        /// </summary>
        public decimal FSettleAmount { get; set; }
        /// <summary>
        /// 结算时总盈利
        /// </summary>
        public decimal FSettleGain { get; set; }
        /// <summary>
        /// 结算是总亏损
        /// </summary>
        public decimal FSettleLoss { get; set; }
        /// <summary>
        /// 分摊额
        /// </summary>
        public decimal FSettleApportion { get; set; }
        /// <summary>
        /// 分摊比
        /// </summary>
        public decimal FSettleRate { get; set; }
    }
}
