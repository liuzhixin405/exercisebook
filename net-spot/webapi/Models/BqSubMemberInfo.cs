using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 批量创建子账号信息
    /// </summary>
    public partial class BqSubMemberInfo
    {
        /// <summary>
        /// 子账号id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public int Dateline { get; set; }
        /// <summary>
        /// 子账号登录名
        /// </summary>
        public string FUserEmail { get; set; } = null!;
        /// <summary>
        /// 子账号密码（未加密）
        /// </summary>
        public string UnencryptedPassword { get; set; } = null!;
        /// <summary>
        /// 访问IP
        /// </summary>
        public string AllowIp { get; set; } = null!;
        /// <summary>
        /// 访问公钥
        /// </summary>
        public string AppKey { get; set; } = null!;
        /// <summary>
        /// 访问私钥
        /// </summary>
        public string AppSecret { get; set; } = null!;
    }
}
