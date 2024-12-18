using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 代币奖励记录
    /// </summary>
    public partial class BqBftRewardRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 币种ID
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 原因（1、每天登陆 2、邀请奖励）
        /// </summary>
        public short Reason { get; set; }
        /// <summary>
        /// 奖励金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
    }
}
