using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 指数状态设置
    /// 
    /// </summary>
    public partial class BqIndexState
    {
        public int Id { get; set; }
        /// <summary>
        /// okex指数状态（1：使用中，0：不使用）
        /// </summary>
        public short OkexState { get; set; }
        /// <summary>
        /// 火币指数状态（1：使用中，0：不使用）
        /// </summary>
        public short HuobiState { get; set; }
        /// <summary>
        /// 币安指数状态（1：使用中，0：不使用）
        /// </summary>
        public short BinanceState { get; set; }
    }
}
