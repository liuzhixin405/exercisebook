using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class VMemberInviteRebateLog
    {
        public uint Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string? BqCode { get; set; }
        /// <summary>
        /// 用户返佣比例
        /// </summary>
        public decimal CommissionRate { get; set; }
        /// <summary>
        /// 返佣比例差
        /// </summary>
        public decimal CommissionRateDiff { get; set; }
        /// <summary>
        /// 返佣
        /// </summary>
        public decimal RebateAmount { get; set; }
        /// <summary>
        /// 返佣时间
        /// </summary>
        public uint RecordTime { get; set; }
        /// <summary>
        /// 下级返佣比例
        /// </summary>
        public decimal SubCommissionRate { get; set; }
        /// <summary>
        /// 下级用户邮箱
        /// </summary>
        public string SubUserEmail { get; set; } = null!;
        /// <summary>
        /// 下级用户id
        /// </summary>
        public int SubUserId { get; set; }
        /// <summary>
        /// 下级用户等级名称
        /// </summary>
        public string SubUserLevelName { get; set; } = null!;
        /// <summary>
        /// 交易id
        /// </summary>
        public uint TradeId { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string UserEmail { get; set; } = null!;
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户等级名称
        /// </summary>
        public string UserLevelName { get; set; } = null!;
        /// <summary>
        /// 币种
        /// </summary>
        public short CurrencyTypeId { get; set; }
    }
}
