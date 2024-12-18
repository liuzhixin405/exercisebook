using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 国家表
    /// </summary>
    public partial class BqAreaCountry
    {
        public ushort Id { get; set; }
        /// <summary>
        /// 国际域名缩写
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// 中文国家名称
        /// </summary>
        public string ZhName { get; set; } = null!;
        /// <summary>
        /// 英文国家名称
        /// </summary>
        public string EnName { get; set; } = null!;
    }
}
