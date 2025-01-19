using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户收藏合约（自选）
    /// </summary>
    public partial class BqMemberFavoriteMarket
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 合约类型：1、正式盘，2、模拟盘
        /// </summary>
        public short Type { get; set; }
        /// <summary>
        /// 合约号(&apos;,&apos;逗号隔开)
        /// </summary>
        public string? Codes { get; set; }
        /// <summary>
        /// 分区
        /// </summary>
        public string Part { get; set; } = null!;
    }
}
