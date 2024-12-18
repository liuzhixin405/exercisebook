using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 记录经纪人推广成功邀请人数统计汇总
    /// </summary>
    public partial class BqMemberInviteCountDay
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 当日成功邀请人数
        /// </summary>
        public uint Count { get; set; }
        /// <summary>
        /// 当天日期(yyyy-mm-dd)
        /// </summary>
        public DateOnly Day { get; set; }
    }
}
