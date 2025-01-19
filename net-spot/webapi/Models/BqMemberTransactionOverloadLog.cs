using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 会员合约爆仓日志(表结构与btc1407_trade_log类似)
    /// </summary>
    public partial class BqMemberTransactionOverloadLog
    {
        public uint Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 合约号ID
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 持仓均价
        /// </summary>
        public decimal TradePrice { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal TradeCount { get; set; }
        /// <summary>
        /// 保证金
        /// </summary>
        public decimal TradeDeposit { get; set; }
        /// <summary>
        /// 爆仓价
        /// </summary>
        public decimal TradeOverloadPrice { get; set; }
        /// <summary>
        /// 强平价
        /// </summary>
        public decimal TradeForcePrice { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal TradeFee { get; set; }
        /// <summary>
        /// 爆仓的方向(1:多单爆仓 2:卖空爆仓)
        /// </summary>
        public bool? Direction { get; set; }
        /// <summary>
        /// 爆仓时间
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 委托挂单表主键id(btc1407_pending_delegation:delegation_id)
        /// </summary>
        public int DelegationId { get; set; }
        /// <summary>
        /// 结余
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// 挂单流水号
        /// </summary>
        public long PendingNo { get; set; }
        /// <summary>
        /// 同持仓表
        /// </summary>
        public decimal FFromremain { get; set; }
    }
}
