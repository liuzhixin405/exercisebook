using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 币币交易-量化用户相关配置
    /// </summary>
    public partial class BqSpotQuantSetting
    {
        public uint Id { get; set; }
        /// <summary>
        /// 量化用户id（逗号隔开）
        /// </summary>
        public string Uids { get; set; } = null!;
        /// <summary>
        /// 是否开启自动撤单
        /// </summary>
        public ulong IsAutoCancel { get; set; }
        /// <summary>
        /// 超时撤单时间（秒）
        /// </summary>
        public int CancelTimeout { get; set; }
        public int? KeepNums { get; set; }
    }
}
