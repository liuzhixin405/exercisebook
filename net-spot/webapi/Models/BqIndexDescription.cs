using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 指数说明表
    /// </summary>
    public partial class BqIndexDescription
    {
        public int Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public long Dateline { get; set; }
        /// <summary>
        /// 中文指数说明
        /// </summary>
        public string ZhDescription { get; set; } = null!;
        /// <summary>
        /// 英文指数说明
        /// </summary>
        public string EnDescription { get; set; } = null!;
        public string? JaDescription { get; set; }
        /// <summary>
        /// 韩文指数说明
        /// </summary>
        public string KoDescription { get; set; } = null!;
        /// <summary>
        /// 繁体指数说明
        /// </summary>
        public string HkDescription { get; set; } = null!;
        /// <summary>
        /// 合约类型code
        /// </summary>
        public string TypeCode { get; set; } = null!;
    }
}
