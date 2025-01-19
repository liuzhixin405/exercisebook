using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 质押借币利率设置
    /// </summary>
    public partial class BqLoanRateSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 借贷配置Id
        /// </summary>
        public int LoanSettingId { get; set; }
        /// <summary>
        /// 周期（天数）
        /// </summary>
        public int Cycle { get; set; }
        /// <summary>
        /// 利率（日）
        /// </summary>
        public decimal DayRate { get; set; }
    }
}
