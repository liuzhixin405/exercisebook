using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户抽奖次数统计表
    /// </summary>
    public partial class BqMemberDraw
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 可抽奖次数
        /// </summary>
        public int FDrawCount { get; set; }
        /// <summary>
        /// 已抽奖次数
        /// </summary>
        public int FAlreadyDrawCount { get; set; }
        /// <summary>
        /// 已产生usdt手续费
        /// </summary>
        public decimal FProduceFeeUsdt { get; set; }
        /// <summary>
        /// 已产生usdr手续费
        /// </summary>
        public decimal FProduceFeeUsdr { get; set; }
        /// <summary>
        /// 取余手续费
        /// </summary>
        public decimal FSurplusFee { get; set; }
        /// <summary>
        /// 收货地址(只有是实物奖励才有收货地址)
        /// </summary>
        public string? Address { get; set; }
    }
}
