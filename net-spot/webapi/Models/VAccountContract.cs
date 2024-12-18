using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约钱包
    /// </summary>
    public partial class VAccountContract
    {
        public int FId { get; set; }
        public int? FUserId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string? FCode { get; set; }
        /// <summary>
        /// 初始btc数量
        /// </summary>
        public decimal? FInitbtc { get; set; }
        /// <summary>
        /// 开仓可用
        /// </summary>
        public decimal? FBtc { get; set; }
        /// <summary>
        /// 合约冻结资金
        /// </summary>
        public decimal? FLockbtc { get; set; }
        /// <summary>
        /// 已结算资金-当前合约持单总盈亏
        /// </summary>
        public decimal? FSettled { get; set; }
    }
}
