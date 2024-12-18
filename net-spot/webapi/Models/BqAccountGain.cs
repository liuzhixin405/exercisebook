using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约资金盈利情况
    /// </summary>
    public partial class BqAccountGain
    {
        public int FGainId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 合约ID
        /// </summary>
        public int TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 该合约盈利的BTC金额
        /// </summary>
        public decimal FBtc { get; set; }
    }
}
