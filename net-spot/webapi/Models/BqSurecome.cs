using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqSurecome
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserPwd { get; set; }
        public string? UserThpwd { get; set; }
        public string? UserRole { get; set; }
        public string? UserSalt { get; set; }
        public string? UserLastip { get; set; }
        public long? UserLastTime { get; set; }
        public string? GoogleNo { get; set; }
        /// <summary>
        /// 密码错误次数
        /// </summary>
        public byte? PwdCount { get; set; }
        /// <summary>
        /// 密码错误最后一次时间
        /// </summary>
        public uint? PwdErrorTime { get; set; }
        /// <summary>
        /// 谷歌验证错误次数
        /// </summary>
        public byte? GoogleCount { get; set; }
        /// <summary>
        /// 谷歌验证错误最后一次时间
        /// </summary>
        public uint? GoogleErrorTime { get; set; }
        /// <summary>
        /// 币币风控，按交易对分配权限
        /// </summary>
        public string? UserCoins { get; set; }
    }
}
