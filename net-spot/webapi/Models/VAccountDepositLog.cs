using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户所持合约保证金不足时，系统自动&quot;追加/返还&quot;保证金记录
    /// </summary>
    public partial class VAccountDepositLog
    {
        public ulong LogId { get; set; }
        /// <summary>
        /// 持仓ID(v_member_transaction:mt_id)
        /// </summary>
        public uint MtId { get; set; }
        /// <summary>
        /// 合约id
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// -1:从可用金额扣除保证金 1:返还保证金到账户可用金额
        /// </summary>
        public bool Direction { get; set; }
        /// <summary>
        /// 变更金额
        /// </summary>
        public decimal Deposit { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public uint Dateline { get; set; }
    }
}
