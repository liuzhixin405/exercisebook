using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 经纪人邀请链接统计
    /// </summary>
    public partial class BqInviteClickCount
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 单日邀请数量
        /// </summary>
        public uint ClickDayCount { get; set; }
        /// <summary>
        /// 周累计
        /// </summary>
        public uint ClickWeekCount { get; set; }
        /// <summary>
        /// 月累计
        /// </summary>
        public uint ClickMonthCount { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public uint ClickTotalCount { get; set; }
        /// <summary>
        /// 当天时间，unix时间格式，只保存yyyy-mm-dd这部分的信息
        /// </summary>
        public uint Day { get; set; }
    }
}
