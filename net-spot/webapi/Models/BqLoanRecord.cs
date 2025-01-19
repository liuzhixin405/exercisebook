using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 质押借币记录
    /// </summary>
    public partial class BqLoanRecord
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
        /// 借出数量
        /// </summary>
        public decimal BorrowAmount { get; set; }
        /// <summary>
        /// 借入到账金额
        /// </summary>
        public decimal ReceivedAmount { get; set; }
        /// <summary>
        /// 抵押数量
        /// </summary>
        public decimal PledgeAmount { get; set; }
        /// <summary>
        /// 抵押率
        /// </summary>
        public decimal PledgeRate { get; set; }
        /// <summary>
        /// 利率（日）
        /// </summary>
        public decimal DayRate { get; set; }
        /// <summary>
        /// 手续费率
        /// </summary>
        public decimal FeeRate { get; set; }
        /// <summary>
        /// 已归还的借出数量
        /// </summary>
        public decimal BackBorrowAmount { get; set; }
        /// <summary>
        /// 已释放的抵押数量
        /// </summary>
        public decimal BackPledgeAmount { get; set; }
        /// <summary>
        /// 已付利息
        /// </summary>
        public decimal Interest { get; set; }
        /// <summary>
        /// 已产生的手续费
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 抵偿本金的质押数量
        /// </summary>
        public decimal PaymentPledgeAmount { get; set; }
        /// <summary>
        /// 抵偿利息的质押数量
        /// </summary>
        public decimal PaymentInterest { get; set; }
        /// <summary>
        /// 借贷时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 抵偿的手续费
        /// </summary>
        public decimal PaymentFee { get; set; }
        /// <summary>
        /// 周期（天数）
        /// </summary>
        public int Cycle { get; set; }
        /// <summary>
        /// 状态（0-提交，1-借款中，2-已还款，3-已平仓
        /// </summary>
        public short State { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
    }
}
