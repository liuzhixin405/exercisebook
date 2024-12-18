using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 抽奖设置表
    /// </summary>
    public partial class BqLuckDrawSetting
    {
        /// <summary>
        /// 奖品ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 奖项(first,second )
        /// </summary>
        public string Prize { get; set; } = null!;
        /// <summary>
        /// 中文名称(一等奖)
        /// </summary>
        public string NameZhCn { get; set; } = null!;
        /// <summary>
        /// 英文名称(firstprize)
        /// </summary>
        public string NameEnUs { get; set; } = null!;
        /// <summary>
        /// 中奖概率
        /// </summary>
        public double Probability { get; set; }
        /// <summary>
        /// 实物奖品
        /// </summary>
        public string PrizeMatter { get; set; } = null!;
        /// <summary>
        /// USDT奖品
        /// </summary>
        public decimal PrizeUsdt { get; set; }
        /// <summary>
        /// 奖品说明
        /// </summary>
        public string? Notes { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string PhotoName { get; set; } = null!;
    }
}
