using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 记录经纪人推广成功邀请人数月统计汇总
    /// </summary>
    public partial class BqMemberInviteCountMonth
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 当月邀请人数汇总
        /// </summary>
        public uint Count { get; set; }
        /// <summary>
        /// 月份(数字类型:201405)
        /// </summary>
        public uint Month { get; set; }
    }
}
