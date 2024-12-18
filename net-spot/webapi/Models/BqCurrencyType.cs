using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 货币类型
    /// </summary>
    public partial class BqCurrencyType
    {
        public byte TypeId { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string TypeName { get; set; } = null!;
        /// <summary>
        /// 中文币种描述
        /// </summary>
        public string? DescriptionZh { get; set; }
        /// <summary>
        /// 状态（1：使用中，0：不使用）
        /// </summary>
        public short State { get; set; }
        public decimal RechargeFee { get; set; }
        public decimal MaxWidthdraw { get; set; }
        public decimal DayMaxWidthdraw { get; set; }
        /// <summary>
        /// 单笔提币需人工审核额度
        /// </summary>
        public decimal ManulMaxWidthdraw { get; set; }
        public decimal WithdrawFee { get; set; }
        /// <summary>
        /// 单笔最少提币数
        /// </summary>
        public decimal MinWithdraw { get; set; }
        /// <summary>
        /// 英文币种描述
        /// </summary>
        public string? DescriptionEn { get; set; }
        /// <summary>
        /// 日文描述
        /// </summary>
        public string? DescriptionJp { get; set; }
        /// <summary>
        /// 韩文描述
        /// </summary>
        public string? DescriptionKr { get; set; }
        /// <summary>
        /// 繁体描述
        /// </summary>
        public string? DescriptionHk { get; set; }
        public ulong Erc20 { get; set; }
        public ulong Trc20 { get; set; }
        /// <summary>
        /// erc20地址提币手续费
        /// </summary>
        public decimal WithdrawFeeErc20 { get; set; }
        /// <summary>
        /// erc20地址充值手续费
        /// </summary>
        public decimal RechargeFeeErc20 { get; set; }
        /// <summary>
        /// trc20地址提币手续费
        /// </summary>
        public decimal WithdrawFeeTrc20 { get; set; }
        /// <summary>
        /// trc20地址充值手续费
        /// </summary>
        public decimal RechargeFeeTrc20 { get; set; }
        /// <summary>
        /// 站内提币手续费
        /// </summary>
        public decimal InsideWithdrawFee { get; set; }
        /// <summary>
        /// 站内充值手续费
        /// </summary>
        public decimal InsideRechargeFee { get; set; }
        /// <summary>
        /// 是否开启充值
        /// </summary>
        public ulong EnableRecharge { get; set; }
        /// <summary>
        /// 是否开启提币
        /// </summary>
        public ulong EnableWithdraw { get; set; }
    }
}
