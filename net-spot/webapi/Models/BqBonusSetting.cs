using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 分红设置
    /// </summary>
    public partial class BqBonusSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 状态（0-停止，1-运行中）
        /// </summary>
        public ulong Running { get; set; }
        /// <summary>
        /// 分红开始时间
        /// </summary>
        public int BeginTime { get; set; }
        public ulong FirstDone { get; set; }
    }
}
