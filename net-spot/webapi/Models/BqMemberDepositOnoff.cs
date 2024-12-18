using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户保证金自动追加开关
    /// </summary>
    public partial class BqMemberDepositOnoff
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 0:未启用(记录不存在时也表示未启用) 1:启用
        /// </summary>
        public ushort Value { get; set; }
    }
}
