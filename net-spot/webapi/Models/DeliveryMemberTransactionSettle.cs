using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 会员合约结算历史表,记录一个合约在结算的时候，所有持有此合约会员的盈亏信息(比持仓表多一个盈亏和分推字段)
    /// </summary>
    public partial class DeliveryMemberTransactionSettle
    {
        public uint Id { get; set; }
        /// <summary>
        /// 合约id
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 成交类型(1:多单开仓/2:多单平仓/3:空头开仓/4:空头平仓/5:多单爆仓/6:空单爆仓)
        /// </summary>
        public sbyte TradeTypeId { get; set; }
        /// <summary>
        /// 杠杆
        /// </summary>
        public ushort TradeMultiple { get; set; }
        /// <summary>
        /// 持仓量
        /// </summary>
        public decimal HoldCount { get; set; }
        /// <summary>
        /// 冻结量
        /// </summary>
        public decimal LockCount { get; set; }
        /// <summary>
        /// 合约均价
        /// </summary>
        public decimal AveragePrice { get; set; }
        /// <summary>
        /// 保证金
        /// </summary>
        public decimal Deposit { get; set; }
        /// <summary>
        /// 盈亏金额
        /// </summary>
        public decimal ProfitLost { get; set; }
        /// <summary>
        /// 分推金额
        /// </summary>
        public decimal Apportion { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        public uint Dateline { get; set; }
        public decimal TradeFee { get; set; }
    }
}
