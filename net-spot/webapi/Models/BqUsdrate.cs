using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqUsdrate
    {
        public int Id { get; set; }
        /// <summary>
        /// 货币类型(1 美元对人民币)
        /// </summary>
        public short? ExType { get; set; }
        /// <summary>
        /// 美元对人民币的汇率 1美元=?人民币
        /// </summary>
        public decimal? ExRate { get; set; }
        public int? ExTime { get; set; }
        /// <summary>
        /// 当前是使用汇率 1为使用, 0 为未使用
        /// </summary>
        public bool? ExUse { get; set; }
    }
}
