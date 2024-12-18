using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberBindBtc100
    {
        public uint FId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// btc100网站对应的uid
        /// </summary>
        public ulong BtcUid { get; set; }
        /// <summary>
        /// btc100用户名
        /// </summary>
        public string BtcUserName { get; set; } = null!;
        /// <summary>
        /// 关系绑定时间,unix时间格式
        /// </summary>
        public uint BindTime { get; set; }
        /// <summary>
        /// 绑定状态
        /// </summary>
        public bool BindState { get; set; }
    }
}
