using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 币币账户与OTC账户互转，人工处理记录
    /// </summary>
    public partial class BqTransferWithOtcRecord
    {
        public uint Id { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string CurrencyName { get; set; } = null!;
        /// <summary>
        /// 币种id
        /// </summary>
        public int CurrencyTypeId { get; set; }
        /// <summary>
        /// 方向(1：转入，2：转出)
        /// </summary>
        public int Direction { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 冻结数量
        /// </summary>
        public decimal FrozenAmount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
        /// <summary>
        /// 操作时间
        /// </summary>
        public int OperTime { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 状态(1：提交，人工处理，2：成功，3：失败)
        /// </summary>
        public short State { get; set; }
    }
}
