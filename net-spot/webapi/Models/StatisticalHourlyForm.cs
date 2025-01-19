using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 每小时统计网站数据
    /// </summary>
    public partial class StatisticalHourlyForm
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// 该合约可用总额
        /// </summary>
        public decimal ContractBalance { get; set; }
        /// <summary>
        /// 该合约保证金总量
        /// </summary>
        public decimal DepositMount { get; set; }
        /// <summary>
        /// 该合约持仓量
        /// </summary>
        public decimal HoldMount { get; set; }
        /// <summary>
        /// 每小时登录用户数量
        /// </summary>
        public int HourlyLogin { get; set; }
        /// <summary>
        /// 每小时注册用户数量
        /// </summary>
        public int HourlyRegister { get; set; }
        /// <summary>
        /// 统计时间（前一个小时的时间点）
        /// </summary>
        public int StatisticalTime { get; set; }
        /// <summary>
        /// 手续费总额
        /// </summary>
        public decimal TotalFee { get; set; }
        /// <summary>
        /// 总注册人数
        /// </summary>
        public int TotalRegister { get; set; }
        /// <summary>
        /// 总交易人数
        /// </summary>
        public int TotalTradeUser { get; set; }
        /// <summary>
        /// 该合约真实交易总量
        /// </summary>
        public decimal TradeCount { get; set; }
        /// <summary>
        /// 该合约交易手续费金额
        /// </summary>
        public decimal TradeFee { get; set; }
        /// <summary>
        /// 该合约交易人数
        /// </summary>
        public int TradeUser { get; set; }
        /// <summary>
        /// 单个合约返佣金额
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 总返佣金额
        /// </summary>
        public decimal TotalRebate { get; set; }
        /// <summary>
        /// bfx返佣总额
        /// </summary>
        public decimal TotalBfxRebate { get; set; }
        /// <summary>
        /// bfx抵扣手续费
        /// </summary>
        public decimal TradeBfxFee { get; set; }
        /// <summary>
        /// bfx抵扣手续费总额
        /// </summary>
        public decimal TotalBfxFee { get; set; }
        /// <summary>
        /// bfx返利
        /// </summary>
        public decimal BfxRebate { get; set; }
    }
}
