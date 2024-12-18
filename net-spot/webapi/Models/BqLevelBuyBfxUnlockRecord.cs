using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 会员购买vip，锁仓Bfx解锁记录
    /// </summary>
    public partial class BqLevelBuyBfxUnlockRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 解锁比例
        /// </summary>
        public decimal UnlockRate { get; set; }
        /// <summary>
        /// 解锁量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 解锁记录时间
        /// </summary>
        public int RecordTime { get; set; }
        /// <summary>
        /// 购买vip的记录id
        /// </summary>
        public int LevelBuyRecordId { get; set; }
        /// <summary>
        /// vip购买记录对应锁仓量
        /// </summary>
        public decimal TotalLock { get; set; }
    }
}
