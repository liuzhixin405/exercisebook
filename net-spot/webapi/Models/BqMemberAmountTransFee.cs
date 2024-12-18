using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 交易的第二个操作，会扣除手续费,后期用来代理商和经纪人分成
    /// </summary>
    public partial class BqMemberAmountTransFee
    {
        public uint RecordId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 标题说明
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 与其交易ID (trade_log:trade_id)
        /// </summary>
        public uint TradeId { get; set; }
        /// <summary>
        /// 时间,unix时间格式
        /// </summary>
        public uint Dateline { get; set; }
    }
}
