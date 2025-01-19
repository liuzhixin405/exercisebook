using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 平台分红记录
    /// </summary>
    public partial class BqBonusRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 分红比例（一个币可分多少钱）
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 分红金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 当天总手续费
        /// </summary>
        public decimal TotalFee { get; set; }
        /// <summary>
        /// 当天总挖币量+IEO解冻
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 用户平台币币量
        /// </summary>
        public decimal HoldAmount { get; set; }
        /// <summary>
        /// 用户平台币挂单冻结（币币交易挂单）
        /// </summary>
        public decimal FrozenAmount { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string Day { get; set; } = null!;
        /// <summary>
        /// 记录时间
        /// </summary>
        public int RecordTime { get; set; }
        public string Note { get; set; } = null!;
    }
}
