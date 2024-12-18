using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 会员申请的交易api
    /// </summary>
    public partial class BqMemberPartner
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 开通api时间
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 商户标识
        /// </summary>
        public string Partner { get; set; } = null!;
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; } = null!;
    }
}
