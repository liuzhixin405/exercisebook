using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 止损表
    /// </summary>
    public partial class BqStopLoss
    {
        public uint Id { get; set; }
        /// <summary>
        /// 合约号ID
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 1止盈 2止损
        /// </summary>
        public short StopLossTypeId { get; set; }
        /// <summary>
        /// 持仓ID
        /// </summary>
        public int TransactionMemberId { get; set; }
        /// <summary>
        /// 止损价
        /// </summary>
        public decimal StopLossPrice { get; set; }
        /// <summary>
        /// 委托成交
        /// </summary>
        public decimal PendingTransPrice { get; set; }
        /// <summary>
        /// 1 买多, 2卖空
        /// </summary>
        public short TradeTypeId { get; set; }
        /// <summary>
        /// 添加记录时间
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 挂单类型（0：限价委托;1：市价委托）
        /// </summary>
        public short OrderType { get; set; }
        /// <summary>
        /// 挂单数量
        /// </summary>
        public decimal PendingAmount { get; set; }
    }
}
