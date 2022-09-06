
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Contract.Core.Enum
{
    public enum MarginMode
    {
        /// <summary>
        /// 全仓保证金模式
        /// </summary>
        [Description("全仓保证金模式")]
        Cross = 0,
        /// <summary>
        ///  逐仓保证金模式
        /// </summary>
        [Description("逐仓保证金模式")]
        Isolated = 1
    }
}
