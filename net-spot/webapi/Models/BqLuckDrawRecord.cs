using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户抽奖记录
    /// </summary>
    public partial class BqLuckDrawRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 奖项
        /// </summary>
        public string Prize { get; set; } = null!;
        /// <summary>
        /// 奖品名称
        /// </summary>
        public string? PrizeName { get; set; }
        /// <summary>
        /// 抽奖时间
        /// </summary>
        public long DrawTime { get; set; }
    }
}
