using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 申请区域代理商时，要代理的区域省份信息
    /// </summary>
    public partial class BqMemberApplyAgentArea
    {
        /// <summary>
        /// 申请用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 省份代码
        /// </summary>
        public string ProvinceCode { get; set; } = null!;
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; } = null!;
        /// <summary>
        /// 国家代码
        /// </summary>
        public string CountryCode { get; set; } = null!;
        /// <summary>
        /// 国家名称
        /// </summary>
        public string CountryName { get; set; } = null!;
        /// <summary>
        /// 1:通过审核 0:待审 -1:拒绝
        /// </summary>
        public bool State { get; set; }
    }
}
