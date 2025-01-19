using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// ieo充提记录
    /// </summary>
    public partial class BqIeoRechargeRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 币种id
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 操作类型(1:充值/2:提现)
        /// </summary>
        public short Operation { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 已解锁
        /// </summary>
        public decimal UnlockAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 更新时间，取消提币，直接减少金额，方便ieo解锁
        /// </summary>
        public int UpdateTime { get; set; }
    }
}
