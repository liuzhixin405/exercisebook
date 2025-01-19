using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 会员信息所在区域登记表
    /// </summary>
    public partial class BqMemberArea
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 国家代码
        /// </summary>
        public string CountryCode { get; set; } = null!;
        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; } = null!;
        /// <summary>
        /// 省份代码
        /// </summary>
        public string ProvinceCode { get; set; } = null!;
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; } = null!;
        /// <summary>
        /// 城市代码
        /// </summary>
        public string CityCode { get; set; } = null!;
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; } = null!;
    }
}
