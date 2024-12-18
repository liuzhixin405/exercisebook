using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 委托挂单表（非全部成交）
    /// </summary>
    public partial class BqTemplatePendingDelegationCache
    {
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
        /// 操作方向
        /// </summary>
        public bool Direction { get; set; }
        /// <summary>
        /// 委托数量
        /// </summary>
        public decimal DelegationCount { get; set; }
        /// <summary>
        /// 委托价格
        /// </summary>
        public decimal DelegationPrice { get; set; }
        /// <summary>
        /// 已成交数量
        /// </summary>
        public decimal DealCount { get; set; }
        /// <summary>
        /// 未成交数量
        /// </summary>
        public decimal UndealCount { get; set; }
        /// <summary>
        /// 杠杆
        /// </summary>
        public ushort Multiple { get; set; }
        /// <summary>
        /// 保证金
        /// </summary>
        public decimal Deposit { get; set; }
        /// <summary>
        /// 挂单成交优先级(1:开仓, 2:平仓, 3:爆仓)
        /// </summary>
        public byte? Priority { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 0默认，1交易中，2撤单中（其他）
        /// </summary>
        public short Flag { get; set; }
        /// <summary>
        /// 保证金来源于余额
        /// </summary>
        public decimal FFrozenremain { get; set; }
        public string CoinPairName { get; set; } = null!;
        public long CoinPairId { get; set; }
        public decimal CommissionRate { get; set; }
    }
}
