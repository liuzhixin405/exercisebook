using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberInviteAmount
    {
        public int FUserId { get; set; }
        /// <summary>
        /// 日点击量
        /// </summary>
        public int FDayClick { get; set; }
        /// <summary>
        /// 日成功量
        /// </summary>
        public int FDaySuccess { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public int FDate { get; set; }
    }
}
