using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 平台套利系统设置
    /// </summary>
    public partial class VArbitrageSetting
    {
        public uint Id { get; set; }
        /// <summary>
        /// 合约号
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
        /// 自动对平，当有对手仓的情况下，及时以买一或卖一价对平
        /// </summary>
        public ulong AutoTrade { get; set; }
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
        /// 价格增量
        /// </summary>
        public decimal DepthPriceGap { get; set; }
        /// <summary>
        /// 数量最小增量
        /// </summary>
        public decimal DepthAmountGapMin { get; set; }
        /// <summary>
        /// 数量最大增量
        /// </summary>
        public decimal DepthAmountGapMax { get; set; }
        /// <summary>
        /// 指数偏离（小数）
        /// </summary>
        public decimal DepthIndexRange { get; set; }
        /// <summary>
        /// 深度数量
        /// </summary>
        public short DepthSize { get; set; }
        /// <summary>
        /// 价格精度
        /// </summary>
        public short PriceDigits { get; set; }
        /// <summary>
        /// 数量精度
        /// </summary>
        public short AmountDigits { get; set; }
        /// <summary>
        /// 是否可自成交
        /// </summary>
        public ulong CanTradeSelf { get; set; }
        /// <summary>
        /// 下单速度（值*300ms）
        /// </summary>
        public short SpeedType { get; set; }
        /// <summary>
        /// 深度量增量（乘）
        /// </summary>
        public decimal AmountMutil { get; set; }
    }
}
