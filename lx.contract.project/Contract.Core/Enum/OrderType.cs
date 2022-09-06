using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Contract.Core.Enum
{
         /// <summary>
         /// 交易类型
         /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 强制平仓
        /// </summary>
        [Description("强制平仓")]
        CompulsoryLiquidation = 2,
        /// <summary>
        /// 强制减仓
        /// </summary>
        [Description("强制减仓")]
        ForcedPositionReduction = 3,
        /// <summary>
        /// 自动减仓
        /// </summary>
        [Description("自动减仓")]
        AutomaticPositionReduction = 4,
        /// <summary>
        /// 市场价
        /// </summary>
        [Description("市场价")]
        Market = 5,
        /// <summary>
        /// 限价
        /// </summary>
        [Description("限价")]
        Limit = 6,
        /// <summary>
        /// 止损
        /// </summary>
        [Description("止损")]
        StopLoss = 7,
        /// <summary>
        /// 止盈
        /// </summary>
        [Description("止盈")]
        StopProfit = 8,
        /// <summary>
        /// 计划单
        /// </summary>
        [Description("计划单")]
        Plan = 9

    }
}