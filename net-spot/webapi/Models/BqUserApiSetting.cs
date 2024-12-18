using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// api延迟设置
    /// </summary>
    public partial class BqUserApiSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 延迟时间(毫秒)
        /// </summary>
        public int DelayTime { get; set; }
        /// <summary>
        /// 是否开启(默认关闭)
        /// </summary>
        public ulong IsOpen { get; set; }
    }
}
