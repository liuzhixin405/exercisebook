using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 区域省份表
    /// </summary>
    public partial class BqAreaProvince
    {
        public int Id { get; set; }
        /// <summary>
        /// 省份代码
        /// </summary>
        public string Code { get; set; } = null!;
        /// <summary>
        /// 省份名称
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// 国家缩写代码
        /// </summary>
        public string CountryCode { get; set; } = null!;
    }
}
