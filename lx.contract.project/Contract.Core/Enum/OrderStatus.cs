using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Contract.Core.Enum
{
    /// <summary>
    /// 单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 等待成交
        /// </summary>
        [Description("挂单")]
        Pending = 0,
        /// <summary>
        /// 成交
        /// </summary>
        [Description("成交")]
        Completed = 1,
        /// <summary>
        /// 取消交易
        /// </summary> 
        [Description("已撤消")]
        Cancel = 2,
        /// <summary>
        /// 无效
        /// </summary>
        [Description("无效单")]
        Invalid = 3,
    }
}
