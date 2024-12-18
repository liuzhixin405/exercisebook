using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 每小时统计模拟盘交易数据
    /// </summary>
    public partial class VStatisticalHourlyForm
    {
        public int Id { get; set; }
        /// <summary>
        /// 模拟交易-合约号
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// 统计时间
        /// </summary>
        public int StatisticalTime { get; set; }
        /// <summary>
        /// 模拟交易-合约可用总额
        /// </summary>
        public decimal VContractBalance { get; set; }
        /// <summary>
        /// 模拟交易-合约保证金总量
        /// </summary>
        public decimal VDepositMount { get; set; }
        /// <summary>
        /// 模拟交易-持仓数量
        /// </summary>
        public decimal VHoldMount { get; set; }
        /// <summary>
        /// 模拟交易-手续费总额
        /// </summary>
        public decimal VTotalFee { get; set; }
        /// <summary>
        /// 模拟交易-所有合约总交易人数
        /// </summary>
        public int VTotalTradeUser { get; set; }
        /// <summary>
        /// 模拟交易-真实交易总量
        /// </summary>
        public decimal VTradeCount { get; set; }
        /// <summary>
        /// 模拟交易-手续费金额
        /// </summary>
        public decimal VTradeFee { get; set; }
        /// <summary>
        /// 模拟交易-交易人数
        /// </summary>
        public int VTradeUser { get; set; }
    }
}
