using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// api接口AppKey
    /// </summary>
    public partial class BqUserAppKey
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        public string AppKey { get; set; } = null!;
        public string AppSecret { get; set; } = null!;
        /// <summary>
        /// 允许的ip列表
        /// </summary>
        public string AllowIp { get; set; } = null!;
        /// <summary>
        /// 更新时间
        /// </summary>
        public int UpdateTime { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int OverTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public short State { get; set; }
    }
}
