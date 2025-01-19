using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 母子账户划转钱包记录
    /// </summary>
    public partial class BqSubMemberTransferLog
    {
        public int Id { get; set; }
        /// <summary>
        /// 划转数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string CurrencyName { get; set; } = null!;
        /// <summary>
        /// 币种id
        /// </summary>
        public sbyte CurrencyTypeId { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public int Dateline { get; set; }
        /// <summary>
        /// 母账户用户名
        /// </summary>
        public string MotherUserEmail { get; set; } = null!;
        /// <summary>
        /// 母账户id
        /// </summary>
        public int MotherUserId { get; set; }
        /// <summary>
        /// 操作类型(1:母账户转入子账户/2:子账户转出到母账户)
        /// </summary>
        public bool Operation { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; } = null!;
        /// <summary>
        /// 子账户用户名
        /// </summary>
        public string SubUserEmail { get; set; } = null!;
        /// <summary>
        /// 子账户id
        /// </summary>
        public int SubUserId { get; set; }
    }
}
