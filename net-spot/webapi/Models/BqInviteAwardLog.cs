using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 经纪人邀请奖励日志，每笔交易都会进行奖励。
    /// </summary>
    public partial class BqInviteAwardLog
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 用户ID(自由经纪人uid或者区域代理uid)
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 标题(奖励原因)
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 佣金比例
        /// </summary>
        public decimal CommissionRate { get; set; }
        /// <summary>
        /// 奖励金额(佣金*佣金比例)
        /// </summary>
        public decimal AwardAmount { get; set; }
        /// <summary>
        /// 奖励日期,unix时间格式
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 下线用户uid
        /// </summary>
        public int Uid { get; set; }
        /// <summary>
        /// 下线用户邮箱
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// 交易ID(trade_log:trade_id)
        /// </summary>
        public uint TradeId { get; set; }
    }
}
