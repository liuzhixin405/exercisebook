using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 挖矿记录
    /// </summary>
    public partial class BqMineRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 挖矿币种
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string TradeType { get; set; } = null!;
        /// <summary>
        /// 交易id
        /// </summary>
        public int TradeId { get; set; }
        /// <summary>
        /// 交易额
        /// </summary>
        public decimal TradeAmount { get; set; }
        /// <summary>
        /// 交易手续费
        /// </summary>
        public decimal TradeFee { get; set; }
        /// <summary>
        /// 挖矿比例
        /// </summary>
        public decimal MineRate { get; set; }
        /// <summary>
        /// 挖币量
        /// </summary>
        public decimal MineAmount { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 交易模式：1-永续合约，2-现货
        /// </summary>
        public int TradeMode { get; set; }
    }
}
