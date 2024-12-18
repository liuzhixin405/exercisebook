using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 币币套利系统设置
    /// </summary>
    public partial class BqSpotArbitrageSetting
    {
        public long Id { get; set; }
        /// <summary>
        /// 交易对
        /// </summary>
        public string? FCode { get; set; }
        /// <summary>
        /// 深度用户
        /// </summary>
        public int DepthUid { get; set; }
        /// <summary>
        /// 深度数据获取路径
        /// </summary>
        public string DepthUrl { get; set; } = null!;
        /// <summary>
        /// 备用深度路径
        /// </summary>
        public string SpareDepthUrl { get; set; } = null!;
        /// <summary>
        /// 深度买单价格调整幅度（%）
        /// </summary>
        public decimal DepthBuyRange { get; set; }
        /// <summary>
        /// 深度卖单价格调整幅度（%）
        /// </summary>
        public decimal DepthSellRange { get; set; }
        /// <summary>
        /// 所允许的深度最小单
        /// </summary>
        public decimal DepthAmountMin { get; set; }
        public decimal DepthAmountMax { get; set; }
        /// <summary>
        /// 深度系统状态(0-停止，1-开启)
        /// </summary>
        public ulong Running { get; set; }
        /// <summary>
        /// 刷单系统状态（0-停止，1-开启）
        /// </summary>
        public ulong Scalping { get; set; }
        /// <summary>
        /// 刷单频率最小（笔数/每分钟）
        /// </summary>
        public int ScalpingFrequencyMin { get; set; }
        /// <summary>
        /// 刷单频率最大（笔数/每分钟）
        /// </summary>
        public int ScalpingFrequencyMax { get; set; }
        /// <summary>
        /// 刷单每笔最小量（在最小和最大之间随机）
        /// </summary>
        public decimal ScalpingMinEach { get; set; }
        /// <summary>
        /// 刷单每笔最大量（在最小和最大之间随机）
        /// </summary>
        public decimal ScalpingMaxEach { get; set; }
        /// <summary>
        /// 外部市场对冲
        /// </summary>
        public ulong Hedging { get; set; }
        /// <summary>
        /// 外部对冲买单价格调整幅度（%）
        /// </summary>
        public decimal HedgingBuyRange { get; set; }
        /// <summary>
        /// 外部对冲卖单价格调整幅度（%）
        /// </summary>
        public decimal HedgingSellRange { get; set; }
        /// <summary>
        /// 指数范围
        /// </summary>
        public decimal ScalpingIndexRange { get; set; }
        /// <summary>
        /// 盘口宽度限制
        /// </summary>
        public decimal ScalpingDepthRange { get; set; }
        /// <summary>
        /// 当前使用深度（0：主用深度，1：备用深度）
        /// </summary>
        public short CurrUseState { get; set; }
        /// <summary>
        /// 风险控制（根据对冲账户金额限制交易）
        /// </summary>
        public ulong RiskManage { get; set; }
        /// <summary>
        /// 买盘价格增量
        /// </summary>
        public decimal DepthBuyPriceGap { get; set; }
        /// <summary>
        /// 卖盘价格增量
        /// </summary>
        public decimal DepthSellPriceGap { get; set; }
        /// <summary>
        /// 买盘数量额外叠加最小量
        /// </summary>
        public decimal DepthBuyAmountOtherMin { get; set; }
        /// <summary>
        /// 卖盘数量额外叠加最小量
        /// </summary>
        public decimal DepthSellAmountOtherMin { get; set; }
        /// <summary>
        /// 买盘数量额外叠加最大量
        /// </summary>
        public decimal DepthBuyAmountOtherMax { get; set; }
        /// <summary>
        /// 卖盘数量额外叠加最大量
        /// </summary>
        public decimal DepthSellAmountOtherMax { get; set; }
        /// <summary>
        /// 真实买盘数量最小量
        /// </summary>
        public decimal DepthBuyAmountMin { get; set; }
        /// <summary>
        /// 真实卖盘数量最小量
        /// </summary>
        public decimal DepthSellAmountMin { get; set; }
        /// <summary>
        /// 真实买盘数量最大量
        /// </summary>
        public decimal DepthBuyAmountMax { get; set; }
        /// <summary>
        /// 真实卖盘数量最大量
        /// </summary>
        public decimal DepthSellAmountMax { get; set; }
        /// <summary>
        /// 买盘反手挂单差价
        /// </summary>
        public decimal DepthBuyBackhandGap { get; set; }
        /// <summary>
        /// 卖盘反手挂单差价
        /// </summary>
        public decimal DepthSellBackhandGap { get; set; }
        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal OpenPrice { get; set; }
        /// <summary>
        /// 是否系统自己挂单(或者引入外部深度)
        /// </summary>
        public ulong DepthSelf { get; set; }
        /// <summary>
        /// 手否反手挂单
        /// </summary>
        public ulong DepthBackhand { get; set; }
        /// <summary>
        /// 是否递增叠加
        /// </summary>
        public ulong DepthIncreasing { get; set; }
        /// <summary>
        /// 是否充值开盘价
        /// </summary>
        public ulong DepthResetOpenPrice { get; set; }
        /// <summary>
        /// 是否可以自成交
        /// </summary>
        public ulong CanTradeSelf { get; set; }
        /// <summary>
        /// 价格精度
        /// </summary>
        public short PriceDigits { get; set; }
        /// <summary>
        /// 数量精度
        /// </summary>
        public short AmountDigits { get; set; }
        /// <summary>
        /// 盘口数量
        /// </summary>
        public short DepthSize { get; set; }
    }
}
