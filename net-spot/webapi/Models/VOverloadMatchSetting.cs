using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 未消耗爆仓单指定账户撮合
    /// </summary>
    public partial class VOverloadMatchSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string FCode { get; set; } = null!;
        /// <summary>
        /// 指定撮合用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public ulong Running { get; set; }
    }
}
