using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 等级购买设置
    /// </summary>
    public partial class BqMemberLevelBuySetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 支付币种（默认为BFX）
        /// </summary>
        public sbyte PayCoinId { get; set; }
        /// <summary>
        /// 支付币种名称（默认为BFX）
        /// </summary>
        public string PayCoinName { get; set; } = null!;
        /// <summary>
        /// 与BFX汇率（1BFX等于多少支付币种）
        /// </summary>
        public decimal ExchangeRate { get; set; }
        /// <summary>
        /// 多少天后开始释放
        /// </summary>
        public int StartUnlockGapDay { get; set; }
        /// <summary>
        /// 总释放次数（分多少次释放完）
        /// </summary>
        public int UnlockTimes { get; set; }
        /// <summary>
        /// 一级上级返佣（直接上级）
        /// </summary>
        public decimal FirstRebate { get; set; }
        /// <summary>
        /// 二级上级返佣（上级的上级）
        /// </summary>
        public decimal SecondRebate { get; set; }
        /// <summary>
        /// 回购指导价格
        /// </summary>
        public decimal BackPrice { get; set; }
    }
}
