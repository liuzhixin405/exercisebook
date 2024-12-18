using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户支付密码，默认用户没有设置密码的时候，不在这里出现记录
    /// </summary>
    public partial class BqMemberPayword
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 支付密码
        /// </summary>
        public string Pwd { get; set; } = null!;
        /// <summary>
        /// 加密salt
        /// </summary>
        public string Salt { get; set; } = null!;
        /// <summary>
        /// 1:启用 0:不启用
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 是否阅读过上证合约用户须知
        /// </summary>
        public bool FReadszms { get; set; }
        public sbyte FailedTimes { get; set; }
    }
}
