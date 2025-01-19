using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户交易白名单（不受风控影响）
    /// </summary>
    public partial class BqUserTradeWhite
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
