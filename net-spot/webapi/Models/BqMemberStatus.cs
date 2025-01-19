using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户信息状态表
    /// </summary>
    public partial class BqMemberStatus
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 用户注册ip
        /// </summary>
        public string RegIp { get; set; } = null!;
        /// <summary>
        /// 最后登录时间,unix时间格式
        /// </summary>
        public uint LastTime { get; set; }
        /// <summary>
        /// 基本后登录ip
        /// </summary>
        public string LastIp { get; set; } = null!;
        public decimal LastComein { get; set; }
    }
}
