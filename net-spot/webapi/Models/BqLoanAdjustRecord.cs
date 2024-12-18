using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 质押金调整历史记录
    /// </summary>
    public partial class BqLoanAdjustRecord
    {
        /// <summary>
        /// 记录id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 借款订单Id
        /// </summary>
        public int LoanRecordId { get; set; }
        /// <summary>
        /// 抵押币种id
        /// </summary>
        public short PledgeCoinId { get; set; }
        /// <summary>
        /// 抵押币种名称
        /// </summary>
        public string PledgeSymbol { get; set; } = null!;
        /// <summary>
        /// 借出币种Id
        /// </summary>
        public short BorrowCoinId { get; set; }
        /// <summary>
        /// 借出币种名称
        /// </summary>
        public string BorrowSymbol { get; set; } = null!;
        /// <summary>
        /// 抵押数量
        /// </summary>
        public decimal PledgeAmount { get; set; }
        /// <summary>
        /// 调整数量
        /// </summary>
        public decimal AdjustAmount { get; set; }
        /// <summary>
        /// 原抵押率
        /// </summary>
        public decimal OldPledgeRate { get; set; }
        /// <summary>
        /// 新抵押率
        /// </summary>
        public decimal NewPledgeRate { get; set; }
        /// <summary>
        /// 调整方向（0、新增，1、追加，-1、减少）
        /// </summary>
        public short Direction { get; set; }
        /// <summary>
        /// 调整时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
    }
}
