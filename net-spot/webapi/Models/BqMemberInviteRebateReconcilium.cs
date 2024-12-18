using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 返利统计表
    /// </summary>
    public partial class BqMemberInviteRebateReconcilium
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string FUserEmail { get; set; } = null!;
        /// <summary>
        /// 返利金额
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 截止时间
        /// </summary>
        public int CutOffTime { get; set; }
    }
}
