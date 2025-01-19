using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 经纪人邀请人数统计,只记录成功的
    /// </summary>
    public partial class BqMemberInviteCount
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 单日邀请数量
        /// </summary>
        public uint DayCount { get; set; }
        /// <summary>
        /// 周累计
        /// </summary>
        public uint WeekCount { get; set; }
        /// <summary>
        /// 月累计
        /// </summary>
        public uint MonthCount { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public uint TotalCount { get; set; }
        /// <summary>
        /// 当天时间，yyyyMMdd
        /// </summary>
        public uint Day { get; set; }
    }
}
