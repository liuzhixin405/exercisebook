using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 快讯
    /// </summary>
    public partial class BqExpressNews
    {
        public int Id { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public int Dateline { get; set; }
        /// <summary>
        /// 英文内容
        /// </summary>
        public string EnContent { get; set; } = null!;
        /// <summary>
        /// 日文内容
        /// </summary>
        public string JaContent { get; set; } = null!;
        /// <summary>
        /// 中文内容
        /// </summary>
        public string ZhContent { get; set; } = null!;
        /// <summary>
        /// 韩文内容
        /// </summary>
        public string KoContent { get; set; } = null!;
        /// <summary>
        /// 繁体内容
        /// </summary>
        public string HkContent { get; set; } = null!;
    }
}
