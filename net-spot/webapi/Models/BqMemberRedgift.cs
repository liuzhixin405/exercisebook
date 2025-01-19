using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberRedgift
    {
        public int FId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? FAmount { get; set; }
        /// <summary>
        /// 类型 1 btc
        /// </summary>
        public short? FType { get; set; }
        /// <summary>
        /// MD5(btc+毫秒级时间戳+5位随机码)
        /// </summary>
        public string? FCode { get; set; }
        /// <summary>
        /// 0 不可用 1 开仓可用 2已使用
        /// </summary>
        public short? FState { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? FUserId { get; set; }
        /// <summary>
        /// 使用时间
        /// </summary>
        public int? FTime { get; set; }
        public long? FPendingcode { get; set; }
    }
}
