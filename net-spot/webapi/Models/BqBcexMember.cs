using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// bcex用户备用表
    /// </summary>
    public partial class BqBcexMember
    {
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string FUserEmail { get; set; } = null!;
        /// <summary>
        /// 用户资金密码
        /// </summary>
        public string FUserPaypwd { get; set; } = null!;
        /// <summary>
        /// 用户登录密码
        /// </summary>
        public string FUserPwd { get; set; } = null!;
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string? FUserTel { get; set; }
    }
}
