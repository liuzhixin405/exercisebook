using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 记录经纪人推广点击周统计汇总
    /// </summary>
    public partial class BqInviteClickCountWeek
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 当周点击汇总
        /// </summary>
        public uint Count { get; set; }
        /// <summary>
        /// 年份，数字类型 2014
        /// </summary>
        public ushort Year { get; set; }
        /// <summary>
        /// 每n周
        /// </summary>
        public byte Week { get; set; }
    }
}
