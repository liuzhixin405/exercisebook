using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户购买VIP等级记录
    /// </summary>
    public partial class BqMemberLevelBuyAmountLog
    {
        public uint LogId { get; set; }
        /// <summary>
        /// 币种（btc/ltc,gtc)1:btc 2:ltc 3...(后期可添加币种)
        /// </summary>
        public byte CurrencyTypeId { get; set; }
        /// <summary>
        /// 增减
        /// </summary>
        public decimal ChangeAmount { get; set; }
        /// <summary>
        /// 操作时间,unix时间格式
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 变更前的等级ID
        /// </summary>
        public sbyte BeforeLevelNameId { get; set; }
        /// <summary>
        /// 变更后的等级ID
        /// </summary>
        public sbyte AfterLevelNameId { get; set; }
        /// <summary>
        /// 变更前的级别名称
        /// </summary>
        public string BeforeLevelName { get; set; } = null!;
        /// <summary>
        /// 变更后的级别名称
        /// </summary>
        public string AfterLevelName { get; set; } = null!;
        /// <summary>
        /// bfx锁定量（买vip送bfx）
        /// </summary>
        public decimal BfxLock { get; set; }
        /// <summary>
        /// bfx已解锁
        /// </summary>
        public decimal BfxUnlock { get; set; }
    }
}
