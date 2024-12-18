using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class ViewInviteAwardCount
    {
        public decimal? DayCount { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
    }
}
