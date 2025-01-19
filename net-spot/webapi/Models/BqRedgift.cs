using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqRedgift
    {
        public int FId { get; set; }
        public decimal? FAmount { get; set; }
        /// <summary>
        /// 红包码 MD5(btc+毫秒级时间戳+5位随机码)
        /// </summary>
        public string? FCode { get; set; }
        /// <summary>
        /// 分发时间
        /// </summary>
        public int? FTime { get; set; }
        /// <summary>
        /// 1 可用 2不可用
        /// </summary>
        public short? FState { get; set; }
        /// <summary>
        /// 分发给经纪人
        /// </summary>
        public int? FUserId { get; set; }
    }
}
