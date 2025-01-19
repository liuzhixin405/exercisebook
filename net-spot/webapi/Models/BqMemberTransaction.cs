using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户合约表/持仓表
    /// </summary>
    public partial class BqMemberTransaction
    {
        /// <summary>
        /// 用户合约挂单号
        /// </summary>
        public uint MtId { get; set; }
        /// <summary>
        /// 合约id
        /// </summary>
        public short TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 成交类型(1:多单开仓,买多/2:空头开仓,卖空)
        /// </summary>
        public string TradeTypeId { get; set; } = null!;
        /// <summary>
        /// 杠杆
        /// </summary>
        public decimal TradeMultiple { get; set; }
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
        /// 追加保证金
        /// </summary>
        public decimal Deposit2 { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 平仓挂单所需的最小亏损备用金
        /// </summary>
        public decimal FMinFrozenremain { get; set; }
        /// <summary>
        /// 平仓挂单亏损超过保证金的挂单量
        /// </summary>
        public decimal FFrozenCount { get; set; }
        /// <summary>
        /// 平仓挂单亏损超过保证金的挂单对应的亏损备用金
        /// </summary>
        public decimal FFrozenremain { get; set; }
        /// <summary>
        /// 保证金来源于余额的量
        /// </summary>
        public decimal FFromremain { get; set; }
        /// <summary>
        /// 持仓总价值
        /// </summary>
        public decimal? FTotalmoney { get; set; }
        /// <summary>
        /// 冻结总价值
        /// </summary>
        public decimal? FLockmoney { get; set; }
        /// <summary>
        /// 用于计算的类型 买1卖-1
        /// </summary>
        public bool? FTypeForjs { get; set; }
        /// <summary>
        /// 0-默认，-1-爆仓操作中，其他操作中
        /// </summary>
        public short Flag { get; set; }
        /// <summary>
        /// flag设置时间
        /// </summary>
        public int FlagTime { get; set; }
    }
}
