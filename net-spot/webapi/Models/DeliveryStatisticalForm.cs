using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class DeliveryStatisticalForm
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal ContractBalance { get; set; }
        public decimal DepositMount { get; set; }
        public decimal HoldMount { get; set; }
        public int StatisticalTime { get; set; }
        public decimal TotalFee { get; set; }
        public int TotalTradeUser { get; set; }
        public decimal TradeCount { get; set; }
        public decimal TradeFee { get; set; }
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
