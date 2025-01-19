using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 数据播报-持仓总量统计设置
    /// </summary>
    public partial class BqDataBroadCastSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 精英持仓人数
        /// </summary>
        public int EliteCount { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string FCode { get; set; } = null!;
        /// <summary>
        /// 放大倍数
        /// </summary>
        public decimal MagnifiedMultiple { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ulong Running { get; set; }
    }
}
