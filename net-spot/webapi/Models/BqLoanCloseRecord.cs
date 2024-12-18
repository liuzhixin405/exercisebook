using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 质押借款平仓记录
    /// </summary>
    public partial class BqLoanCloseRecord
    {
        /// <summary>
        /// 记录Id
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
        /// 借出方用户Id
        /// </summary>
        public int BorrowUid { get; set; }
        /// <summary>
        /// 抵押数量
        /// </summary>
        public decimal PledgeAmount { get; set; }
        /// <summary>
        /// 退回抵押数量
        /// </summary>
        public decimal BackPledgeAmount { get; set; }
        /// <summary>
        /// 抵偿本金
        /// </summary>
        public decimal PaymentPledgeAmount { get; set; }
        /// <summary>
        /// 抵偿利息
        /// </summary>
        public decimal Interest { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 拖欠数量
        /// </summary>
        public decimal DefaultAmount { get; set; }
        /// <summary>
        /// 利率（日）
        /// </summary>
        public decimal DayRate { get; set; }
        /// <summary>
        /// 手续费率
        /// </summary>
        public decimal FeeRate { get; set; }
        /// <summary>
        /// 平仓结果（1、成功，2、失败）
        /// </summary>
        public short CloseResult { get; set; }
        /// <summary>
        /// 价格指数
        /// </summary>
        public decimal PriceIndex { get; set; }
        /// <summary>
        /// 平仓时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
    }
}
