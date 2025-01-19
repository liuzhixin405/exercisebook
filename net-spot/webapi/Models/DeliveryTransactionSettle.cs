using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约结算表公共表(一个合约一条记录)
    /// </summary>
    public partial class DeliveryTransactionSettle
    {
        public ushort Id { get; set; }
        /// <summary>
        /// 合约ID
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 结算价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 成交量
        /// </summary>
        public decimal TransNum { get; set; }
        /// <summary>
        /// 系统盈亏
        /// </summary>
        public decimal SystemProfitLoss { get; set; }
        /// <summary>
        /// 净盈利部分分摊比例
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 结算时间
        /// </summary>
        public uint Dateline { get; set; }
    }
}
