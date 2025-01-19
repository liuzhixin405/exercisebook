using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class ViewUsermanager
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 是否是子账号(0:否,1:是)
        /// </summary>
        public bool IsSubUser { get; set; }
        /// <summary>
        /// 用户VIP级别(bq_member_level:level_id)
        /// </summary>
        public byte FUserLevelId { get; set; }
        public decimal Credits { get; set; }
        /// <summary>
        /// 注册时间,unix时间格式
        /// </summary>
        public uint FUserTime { get; set; }
        /// <summary>
        /// 用户状态（0:无效,1:有效,-1:冻结）
        /// </summary>
        public sbyte FUserState { get; set; }
        /// <summary>
        /// 用户真实邮箱
        /// </summary>
        public string? FUserEmail { get; set; }
        /// <summary>
        /// 用户电话(手机/固话/海外电话),可能手机号认证
        /// </summary>
        public string? FUserTel { get; set; }
        public decimal Btc { get; set; }
        public decimal BtcLocked { get; set; }
        /// <summary>
        /// 国家名称
        /// </summary>
        public string? CountryName { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string? CityName { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        public string? ProvinceName { get; set; }
        public decimal BtcGain { get; set; }
        public decimal InviteUid { get; set; }
    }
}
