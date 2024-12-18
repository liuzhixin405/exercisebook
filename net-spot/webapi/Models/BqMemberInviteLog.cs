using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 经纪人用户邀请关系表
    /// </summary>
    public partial class BqMemberInviteLog
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 受邀请人uid
        /// </summary>
        public uint InviteUid { get; set; }
        /// <summary>
        /// 邀请时间,unix时间格式
        /// </summary>
        public uint InviteTime { get; set; }
    }
}
