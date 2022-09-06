using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Contract.Core.Enum
{
    public enum ContractTradeSide
    {
        /// <summary>
        /// 开多仓
        /// </summary>
        [Description("开多仓")]
        OpenLong,
        /// <summary>
        /// 开空仓
        /// </summary>
        [Description("开空仓")]
        OpenShort,
        /// <summary>
        /// 平空仓
        /// </summary>
        [Description("平空仓")]
        CloseShort,
        /// <summary>
        /// 平多仓
        /// </summary>
        [Description("平多仓")]
        CloseLong
    }
    /// <summary>
    /// 多空方向
    /// </summary>
    public enum ContractSide
    {
        /// <summary>
        /// 开
        /// </summary>
        [Description("多")]
        Long,
        /// <summary>
        /// 空
        /// </summary>
        [Description("空")]
        Short
    }
}
