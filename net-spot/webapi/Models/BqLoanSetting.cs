using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 质押借币配置
    /// </summary>
    public partial class BqLoanSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 抵押币种id
        /// </summary>
        public short PledgeCoinId { get; set; }
        /// <summary>
        /// 抵押币种名称
        /// </summary>
        public string PledgeSymbol { get; set; } = null!;
        /// <summary>
        /// 质押数量小数位数
        /// </summary>
        public short PledgeDigits { get; set; }
        /// <summary>
        /// 借出币种Id
        /// </summary>
        public short BorrowCoinId { get; set; }
        /// <summary>
        /// 借出币种名称
        /// </summary>
        public string BorrowSymbol { get; set; } = null!;
        /// <summary>
        /// 可借出用户列表（逗号分隔）
        /// </summary>
        public string BorrowUids { get; set; } = null!;
        /// <summary>
        /// 借出数量小数位数
        /// </summary>
        public short BorrowDigits { get; set; }
        /// <summary>
        /// 最大借出数量
        /// </summary>
        public decimal MaxBorrowAmount { get; set; }
        /// <summary>
        /// 最少借出数量
        /// </summary>
        public decimal MinBorrowAmount { get; set; }
        /// <summary>
        /// 初始抵押率
        /// </summary>
        public decimal InitPledgeRate { get; set; }
        /// <summary>
        /// 补仓抵押率
        /// </summary>
        public decimal CurrPledgeRate { get; set; }
        /// <summary>
        /// 平仓抵押率
        /// </summary>
        public decimal ClosePledgeRate { get; set; }
        /// <summary>
        /// 手续费率
        /// </summary>
        public decimal FeeRate { get; set; }
        /// <summary>
        /// 状态（0-已关闭，1-进行中）
        /// </summary>
        public short State { get; set; }
        /// <summary>
        /// 借出用户方式（0-顺序，1-随机）
        /// </summary>
        public short BorrowType { get; set; }
    }
}
