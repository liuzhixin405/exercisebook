using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberValidateInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// 证件背面图片路径
        /// </summary>
        public string? CardBackImg { get; set; }
        /// <summary>
        /// 证件到期时间
        /// </summary>
        public string? CardDueTime { get; set; }
        /// <summary>
        /// 证件正面图片路径
        /// </summary>
        public string? CardFrontImg { get; set; }
        /// <summary>
        /// 证件图片
        /// </summary>
        public string? CardImg { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string? CardNo { get; set; }
        public string? CardType { get; set; }
        /// <summary>
        /// 最新认证时间
        /// </summary>
        public int? LastValidateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// 实名认证真实姓名
        /// </summary>
        public string RealName { get; set; } = null!;
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 认证来源
        /// </summary>
        public string? ValidateSource { get; set; }
        /// <summary>
        /// 认证状态
        /// </summary>
        public string? ValidateState { get; set; }
        /// <summary>
        /// 初次认证时间
        /// </summary>
        public int? ValidateTime { get; set; }
    }
}
