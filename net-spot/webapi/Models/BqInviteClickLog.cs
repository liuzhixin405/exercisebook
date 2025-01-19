using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 经纪人推广邀请链接点击记录，此表为只写表
    /// </summary>
    public partial class BqInviteClickLog
    {
        /// <summary>
        /// 日志ID(后期需要分表，不然此表特别的大)
        /// </summary>
        public ulong LogId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 来源url
        /// </summary>
        public string RefererUrl { get; set; } = null!;
        /// <summary>
        /// 点击时间
        /// </summary>
        public int Dateline { get; set; }
        /// <summary>
        /// 点击ip
        /// </summary>
        public string Ip { get; set; } = null!;
    }
}
