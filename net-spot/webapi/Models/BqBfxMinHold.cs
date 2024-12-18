using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户一天内持有bfx最小值
    /// </summary>
    public partial class BqBfxMinHold
    {
        public int Id { get; set; }
        /// <summary>
        /// 币种id
        /// </summary>
        public short CoinId { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 一天内最小持仓
        /// </summary>
        public decimal MinHold { get; set; }
        /// <summary>
        /// 每天0点时间戳
        /// </summary>
        public int DayTime { get; set; }
    }
}
