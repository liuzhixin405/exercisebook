using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 量化用户相关配置
    /// </summary>
    public partial class BqQuantSetting
    {
        public uint Id { get; set; }
        /// <summary>
        /// 分区
        /// </summary>
        public string Part { get; set; } = null!;
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
        /// <summary>
        /// 保留挂单笔数
        /// </summary>
        public short KeepNums { get; set; }
        /// <summary>
        /// 量化用户自动对平
        /// </summary>
        public ulong AutoDuiping { get; set; }
    }
}
