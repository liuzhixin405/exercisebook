using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 总公司交易收益日志(同表 bq_invite_award_log 结构一样)
    /// </summary>
    public partial class BqCompanyAwardLog
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 用户ID
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
