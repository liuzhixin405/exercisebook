using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 申请经纪人(user_type字段为身份识别字段)
    /// </summary>
    public partial class BqMemberApply
    {
        /// <summary>
        /// 申请人uid
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string Idcard { get; set; } = null!;
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; } = null!;
        /// <summary>
        /// 通讯地址
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// 申请时间
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 1:通过审核 0:待审 -1:拒绝(同步bq_member_apply_agent_area)
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public uint ConfirmDateline { get; set; }
        /// <summary>
        /// 1:自由经纪人 2:区域代理商
        /// </summary>
        public byte UserType { get; set; }
        /// <summary>
        /// 资料审核未通过原因(通过后，此字段内容要清空)
        /// </summary>
        public string Reason { get; set; } = null!;
    }
}
