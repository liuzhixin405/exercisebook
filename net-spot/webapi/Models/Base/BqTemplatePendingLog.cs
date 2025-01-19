using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqTemplatePendingLog
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 委托编号
        /// </summary>
        public uint DelegationId { get; set; }
        /// <summary>
        /// 挂单流水号
        /// </summary>
        public long PendingNo { get; set; }
        /// <summary>
        /// 委托时间,unix时间格式
        /// </summary>
        public uint DelegationTime { get; set; }
        /// <summary>
        /// 合约号id
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 操作类型(1:多单开仓/2:多单平仓/3:空头开仓/4:空头平仓)
        /// </summary>
        public bool TradeTypeId { get; set; }
        /// <summary>
        /// 委托数量
        /// </summary>
        public decimal DelegationCount { get; set; }
        /// <summary>
        /// 委托价格
        /// </summary>
        public decimal DelegationPrice { get; set; }
        /// <summary>
        /// 杠杆
        /// </summary>
        public ushort Multiple { get; set; }
        /// <summary>
        /// 已成交数量
        /// </summary>
        public decimal DealCount { get; set; }
        /// <summary>
        /// 未成交数量
        /// </summary>
        public decimal UndealCount { get; set; }
        /// <summary>
        /// 保证金
        /// </summary>
        public decimal Deposit { get; set; }
        /// <summary>
        /// 1:成交/2:结算/3:强平/4:撤单
        /// </summary>
        public bool? Action { get; set; }
        /// <summary>
        /// 操作时间,unix时间格式
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 0表示正常挂单日志，1标识交易异常时转过来的挂单
        /// </summary>
        public short Flag { get; set; }
    }
}
