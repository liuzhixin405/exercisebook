using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 提币和兑换风控
    /// </summary>
    public partial class BqUserRiskControl
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public int AddTime { get; set; }
    }
}
