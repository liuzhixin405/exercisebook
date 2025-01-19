using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// api白名单
    /// </summary>
    public partial class BqUserApiWhite
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public int AddTime { get; set; }
    }
}
