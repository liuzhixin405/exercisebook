using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户登录失败次数统计，可以根据时间字段自行设置多长时间段内用户登录失败次数并进行限制(用户+ip 限制|ip限制)
    /// </summary>
    public partial class BqCommonFailedlogin
    {
        /// <summary>
        /// 失败IP
        /// </summary>
        public string Ip { get; set; } = null!;
        /// <summary>
        /// 失败时的用户名
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// 尝试次数
        /// </summary>
        public byte Count { get; set; }
        /// <summary>
        /// 最后一次尝试时间
        /// </summary>
        public uint Lastupdate { get; set; }
        public string Username { get; set; } = null!;
        /// <summary>
        /// 谷歌验证错误次数
        /// </summary>
        public byte GoogleCount { get; set; }
        /// <summary>
        /// 谷歌验证错误最后一次时间
        /// </summary>
        public uint GoogleLast { get; set; }
    }
}
