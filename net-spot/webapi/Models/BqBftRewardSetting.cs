using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// BFT代币奖励相关设置
    /// </summary>
    public partial class BqBftRewardSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 币种id
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 登录奖励
        /// </summary>
        public decimal LoginReward { get; set; }
        /// <summary>
        /// 邀请奖励
        /// </summary>
        public decimal InviteReward { get; set; }
        /// <summary>
        /// 持有bfx是否已奖励
        /// </summary>
        public ulong BfxRewarded { get; set; }
        /// <summary>
        /// 注册奖励
        /// </summary>
        public decimal RegisterReward { get; set; }
        /// <summary>
        /// 老用户是否已经赠送
        /// </summary>
        public ulong OldRegisterRewarded { get; set; }
    }
}
