using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约系统亏损用户分推表(只有当bq_transaction_settle:system_profit_loss值为负数的时候，才会将参与分推用户的信息记录在这个表里)
    /// </summary>
    public partial class BqTransactionSettleApportionMember
    {
        public uint Id { get; set; }
        /// <summary>
        /// 合约ID
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
    }
}
